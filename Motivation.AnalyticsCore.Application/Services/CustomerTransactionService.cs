using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Services;

public class CustomerTransactionService : IEntityServices<CustomerTransaction, CustomerTransactionDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerTransactionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Operation<List<CustomerTransaction>, string>> CreateOrEditAsync(List<CustomerTransactionDto> models)
    {
        var customerCodes = models
            .GroupBy(x => x.Customer.Code)
            .Select(x => CodeValue.Create(x.Key));
        if (customerCodes.Any(x => !x.Ok))
            return Operation.Error(customerCodes.FirstOrDefault(x => !x.Ok).Error);

        var companyCodes = models
            .GroupBy(x => x.Company.Code)
            .Select(x => CodeValue.Create(x.Key));
        if (companyCodes.Any(x => !x.Ok))
            return Operation.Error(companyCodes.FirstOrDefault(x => !x.Ok).Error);

        var productCodes = models
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
            .ToList();

        var companies = (await _unitOfWork
            .GetRepository<Company>()
            .GetAllAsync(
                predicate: x => companyCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
            .ToList();
        
        var products = (await _unitOfWork
            .GetRepository<Product>()
            .GetAllAsync(
                predicate: x => productCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
            .ToList();

        List<CustomerTransaction> customerTransactions = [];

        foreach (var model in models)
        {
            var company = companies.FirstOrDefault(x => x.Code.Value == model.Company.Code);
            if (company is null)
                return Operation.Error("Company not found!");

            var customer = customers.FirstOrDefault(x => x.Code.Value == model.Customer.Code);
            if (customer is null)
                return Operation.Error("Customer not found!");

            var transactionProducts = products
                .Where(x => model.TransactionProducts.Select(p => p.Product.Code).Contains(x.Code.Value))
                .ToList();

            var customerTransaction = CustomerTransactionMapper.ToEntity(model, company, customer, transactionProducts);
            if (!customerTransaction.Ok)
                return Operation.Error(customerTransaction.Error);
            customerTransactions.Add(customerTransaction.Result);
        }

        _unitOfWork.GetRepository<CustomerTransaction>()
            .Insert(customerTransactions);
        return customerTransactions;
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
}
