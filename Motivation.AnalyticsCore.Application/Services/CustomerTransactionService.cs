using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Services;

public class CustomerTransactionService : IEntityServices<CustomerTransactionDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerTransactionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Operation<bool, string>> CreateOrEditAsync(Dictionary<string, CustomerTransactionDto> models, DateTime dateStart, DateTime dateEnd, CancellationToken cancellationToken)
    {
        var customerTransactionCodes = models.Values
            .GroupBy(x => x.Code)
            .Select(x => CodeValue.Create(x.Key));

        var customer = models.Values.Where(x => x.Customer.Code == "С00000116").ToArray();

        if (customerTransactionCodes.Any(x => !x.Ok))
        {
            return Operation.Error(customerTransactionCodes.FirstOrDefault(x => !x.Ok).Error);
        }
        var saveTransactionCodes = customerTransactionCodes.Select(x => x.Result.Value).ToHashSet();
        var existingCustomerTransaction = (await _unitOfWork
            .GetRepository<CustomerTransaction>()
            .GetAllAsync(
                predicate: x => 
                    x.AsOfDate >= dateStart
                    && x.AsOfDate <= dateEnd
                || customerTransactionCodes.Select(c => c.Result).Contains(x.Code),
                include: query => query
                    .Include(x => x.CustomerTransactionProducts)
                        .ThenInclude(x => x.Product)
                    .Include(x => x.Company)
                    .Include(x => x.Customer),
                trackingType: TrackingType.Tracking))
            .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        var deleteMissingEntities = DeleteAllExceptAsync(existingCustomerTransaction ?? [], saveTransactionCodes);
        if (!deleteMissingEntities.Ok)
        {
            return Operation.Error(deleteMissingEntities.Error);
        }

        var customerCodes = models.Values
            .GroupBy(x => x.Customer.Code)
            .Select(x => CodeValue.Create(x.Key));
        if (customerCodes.Any(x => !x.Ok))
            return Operation.Error(customerCodes.FirstOrDefault(x => !x.Ok).Error);

        var companyCodes = models.Values
            .GroupBy(x => x.Company.Code)
            .Select(x => CodeValue.Create(x.Key));
        if (companyCodes.Any(x => !x.Ok))
            return Operation.Error(companyCodes.FirstOrDefault(x => !x.Ok).Error);

        var productCodes = models.Values
            .SelectMany(x => x.TransactionProducts)
            .GroupBy(x => x.Product.Code)
            .Select(x => CodeValue.Create(x.Key));
        if (productCodes.Any(x => !x.Ok))
            return Operation.Error(productCodes.FirstOrDefault(x => !x.Ok).Error);

        var customers = (await _unitOfWork
            .GetRepository<Customer>()
            .GetAllAsync(
                predicate: x => customerCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
            .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        var companies = (await _unitOfWork
            .GetRepository<Company>()
            .GetAllAsync(
                predicate: x => companyCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
            .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        var products = (await _unitOfWork
            .GetRepository<Product>()
            .GetAllAsync(
                predicate: x => productCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
            .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        var edit = Edit(existingCustomerTransaction ?? [], models, companies, customers, products);
        if (!edit.Ok)
        {
            return Operation.Error(edit.Error);
        }

        var create = Create(existingCustomerTransaction ?? [], models, companies, customers, products);
        if (!edit.Ok)
        {
            return Operation.Error(edit.Error);
        }

        return true;  
    }

    private Operation<bool, string> Create(
    Dictionary<string, CustomerTransaction> existingEntities,
    Dictionary<string, CustomerTransactionDto> models,
    Dictionary<string, Company> companies,
    Dictionary<string, Customer> customers,
    Dictionary<string, Product> products)
    {
        var newCustomerTransactions = models.Values
        .Where(x => !existingEntities.Values.Any(t => t.Code.Value == x.Code));

        List<CustomerTransaction> customerTransactions = [];
        foreach (var model in newCustomerTransactions)
        {
            if (!companies.TryGetValue(model.Company.Code, out var company))
                return Operation.Error("Company not found!");
            if (company is null)
                return Operation.Error("Company is null!");

            if (!customers.TryGetValue(model.Customer.Code, out var customer))
                return Operation.Error("Customer not found!");
            if (customer is null)
                return Operation.Error("Customer is null!");

            var transactionProducts = products.Values
                .Where(x => model.TransactionProducts.Select(p => p.Product.Code).Contains(x.Code.Value))
                .ToDictionary(
                    x => x.Code.Value,
                    x => x,
                    StringComparer.OrdinalIgnoreCase
                );

            var customerTransaction = CustomerTransactionMapper.ToEntity(model, company, customer, transactionProducts);
            if (!customerTransaction.Ok)
                return Operation.Error(customerTransaction.Error);

            customerTransactions.Add(customerTransaction.Result);
        }

        if (newCustomerTransactions.Any())
        {
            _unitOfWork.GetRepository<CustomerTransaction>()
                .Insert(customerTransactions);
        }
        return true;
    }

    public async Task<Operation<bool, string>> DeleteAsync(DateTime dateStart, DateTime dateEnd)
    {
        var customerTransactions = await _unitOfWork
            .GetRepository<CustomerTransaction>()
                .GetAllAsync(
                    predicate: x => x.AsOfDate >= dateStart 
                    && x.AsOfDate<= dateEnd,
                    trackingType: TrackingType.Tracking);

        _unitOfWork.GetRepository<CustomerTransaction>().Delete(customerTransactions);

        return true;
    }
    private Operation<bool, string> Edit(
        Dictionary<string, CustomerTransaction> existingEntities, 
        Dictionary<string, CustomerTransactionDto> models,
        Dictionary<string, Company> companies,
        Dictionary<string, Customer> customers,
        Dictionary<string, Product> products)
    {
        var editCustomerTransaction = existingEntities.Values
            .Where(x => models.Values.Any(t => t.Code == x.Code.Value))
            .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        foreach (var entity in editCustomerTransaction.Values)
        {
            if (!models.TryGetValue(entity.Code.Value, out var model))
                return Operation.Error("Model not found!");
            if (model is null)
                return Operation.Error("Model is null!");

            if (!companies.TryGetValue(model.Company.Code, out var company))
                return Operation.Error("Company not found!");
            if (company is null)
                return Operation.Error("Company is null!");

            if (!customers.TryGetValue(model.Customer.Code, out var customer))
                return Operation.Error("Customer not found!");
            if (customer is null)
                return Operation.Error("Customer is null!");

            var editResult = entity.EditEntity(model, company, customer, products);
            if (!editResult.Ok)
                return Operation.Error(editResult.Error);
        }
        if (editCustomerTransaction.Any())
        {
            _unitOfWork.GetRepository<CustomerTransaction>()
                .Update(editCustomerTransaction.Values);
        }
        return true;
    }

    private Operation<bool, string> DeleteAllExceptAsync( Dictionary<string, CustomerTransaction> existingEntities, HashSet<string> saveCodes) 
    {
        var deleteCustomerTransactions = existingEntities.Values
        .Where(x => !saveCodes.Any(t => t == x.Code.Value))
        .ToDictionary(
            x => x.Code.Value,
            x => x,
            StringComparer.OrdinalIgnoreCase
        );

        if (deleteCustomerTransactions.Any())
        {
            _unitOfWork.GetRepository<CustomerTransaction>()
                .Delete(deleteCustomerTransactions.Values);
        }
        return true;
    }
}
