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
        var customerCodes = 
            models.Values
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

        var newCustomers = models.Values
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

        List<Customer> customers = [];
        foreach (var model in newCustomers.Values)
        {
            var customer = CustomerMapper.ToEntity(model, customerGroups);
            if (!customer)
                return Operation.Error(customer.Error);
            customers.Add(customer.Result);
        }

        var editResult = Edit(models, existingCustomers, customerGroups);
        if (!editResult)
            return Operation.Error(editResult.Error);

        var allCustomers = new List<Customer>();
        allCustomers.AddRange(customers);
        allCustomers.AddRange(existingCustomers.Values);

        var setMasterCustomers = await SetMasterCustomers(models, allCustomers);
        if (!setMasterCustomers)
            return Operation.Error(setMasterCustomers.Error);
        
        if (customers.Any())
        {
            await _unitOfWork.GetRepository<Customer>().InsertAsync(customers);
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

    private async Task<Operation<bool, string>> SetMasterCustomers(Dictionary<string,CustomerDto> models, List<Customer> entities)
    {
        var customerCodes = models.Values
            .Where(x => x.MasterCustomer is not null && !string.IsNullOrWhiteSpace(x.MasterCustomer.Code))
            .Select(x => CodeValue.Create(x.MasterCustomer.Code));

        if (customerCodes.Any(x => !x.Ok))
            return Operation.Error(customerCodes.FirstOrDefault(x => !x.Ok).Error);

        var existingCustomers = (await _unitOfWork
            .GetRepository<Customer>()
            .GetAllAsync(
                predicate: x => customerCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
            .ToList();

        var newEntities = entities.Where(x => !existingCustomers.Select(x=>x.Code).Contains(x.Code));

        existingCustomers.AddRange(newEntities);

        foreach (var entity in entities)
        {
            if (!models.TryGetValue(entity.Code.Value, out var model))
                return Operation.Error("Company not found!");
            if(model is null)
                return Operation.Error($"Model {typeof(CustomerDto).Name} not found!");

            entity.SetMaster(
                existingCustomers.FirstOrDefault(x => x.Code.Value == model.MasterCustomer?.Code));
        }
        return true;
    }
}
