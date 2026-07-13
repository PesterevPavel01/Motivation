using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Services;

public class CustomerService : IEntityServices<CustomerDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Operation<bool, string>> CreateOrEditAsync(Dictionary<string, CustomerDto> models, DateTime dateStart, DateTime dateEnd , CancellationToken cancellationToken)
    {
        var customerCodes = models.Values
            .Select(x => CodeValue.Create(x.Code));

        if (customerCodes.Any(x => !x.Ok))
            return Operation.Error(customerCodes.FirstOrDefault(x => !x.Ok).Error);

        var existingCustomers = (await _unitOfWork
            .GetRepository<Customer>()
            .GetAllAsync(
                predicate: x => customerCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
             .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        var existingCodes = existingCustomers
            .Keys
            .ToHashSet();

        var newCustomerModels = models.Values
            .Where(x => !existingCodes.Contains(x.Code))
            .ToDictionary(
                x => x.Code,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        var customerGroups = (await _unitOfWork
            .GetRepository<CustomerGroup>()
            .GetAllAsync(trackingType: TrackingType.Tracking))
            .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        Dictionary<string, Customer> newCustomers = [];
        foreach (var model in newCustomerModels.Values)
        {
            var customer = CustomerMapper.ToEntity(model, customerGroups);
            if (!customer)
                return Operation.Error(customer.Error);
            newCustomers[customer.Result.Code.Value] = customer.Result;
        }

        var editResult = Edit(models, existingCustomers, customerGroups);
        if (!editResult)
            return Operation.Error(editResult.Error);

        var customers = newCustomers.Values
            .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        foreach (var customer in existingCustomers) 
        {
            customers[customer.Key] = customer.Value;
        }

        var setMasterCustomers = await SetMasterCustomers(models, customers);
        if (!setMasterCustomers)
            return Operation.Error(setMasterCustomers.Error);
        
        if (newCustomers.Any())
        {
            await _unitOfWork.GetRepository<Customer>().InsertAsync(newCustomers.Values);
        }

        if (existingCustomers.Any())
        {
            _unitOfWork.GetRepository<Customer>().Update(existingCustomers.Values);
        }

        return true;
    }

    public Task<Operation<bool, string>> DeleteAsync(DateTime dateStart, DateTime dateEnd)
    {
        throw new NotImplementedException();
    }

    private Operation<bool, string> Edit(Dictionary<string, CustomerDto> models, Dictionary<string, Customer> entities, Dictionary<string, CustomerGroup> customerGroups)
    {
        foreach (var entity in entities.Values)
        {
            if (!models.TryGetValue(entity.Code.Value, out var model))
                return Operation.Error($"Model {typeof(CustomerDto).Name} not found!");

            var editResult = entity.EditEntity(model, customerGroups);
            if (!editResult.Ok)
                return Operation.Error(editResult.Error);
        }
        return true;
    }

    private async Task<Operation<bool, string>> SetMasterCustomers(Dictionary<string,CustomerDto> models, Dictionary<string, Customer> customers)
    {
        var masterCustomerCodes = models.Values
            .Where(x => x.MasterCustomer is not null && !string.IsNullOrWhiteSpace(x.MasterCustomer.Code))
            .Select(x => x.MasterCustomer!.Code)
            .Distinct()
            .Select(CodeValue.Create);

        if (masterCustomerCodes.Any(x => !x.Ok))
            return Operation.Error(masterCustomerCodes.FirstOrDefault(x => !x.Ok).Error);

        var masters = (await _unitOfWork
            .GetRepository<Customer>()
            .GetAllAsync(
                predicate: x => masterCustomerCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
            .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        var newParrents = customers.Values
            .Where(x =>
                masterCustomerCodes.Select(x => x.Result).Contains(x.Code)
                && !masters.Values.Select(x => x.Code).Contains(x.Code))
            .DistinctBy(x => x.Code);

        foreach (var parent in newParrents)
        {
            masters[parent.Code.Value] = parent;
        }

        foreach (var customer in customers.Values)
        {
            if (!models.TryGetValue(customer.Code.Value, out var model))
                return Operation.Error("Model not found!");

            if (model is null)
                return Operation.Error($"Model {typeof(CustomerDto).Name} not found!");

            if (model.MasterCustomer is null || string.IsNullOrWhiteSpace(model.MasterCustomer.Code))
            {
                customer.SetMaster(null);
            }
            else
            {
                if (!masters.TryGetValue(model.MasterCustomer.Code, out var master))
                    return Operation.Error($"Master customer '{model.MasterCustomer!.Code}' not found for group '{customer.Code.Value}'!");
                customer.SetMaster(master);
            }
        }
        return true;
    }
}
