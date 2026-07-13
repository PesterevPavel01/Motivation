using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Services;

public sealed class CustomerGroupService : IEntityServices<CustomerGroupDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public CustomerGroupService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Operation<bool, string>> CreateOrEditAsync(Dictionary<string, CustomerGroupDto> models, DateTime dateStart, DateTime dateEnd, CancellationToken cancellationToken)
    {
        var customerGroupCodes =
            models.Values
            .Select(x => CodeValue.Create(x.Code));

        var existingCustomerGroups = (await _unitOfWork
            .GetRepository<CustomerGroup>()
            .GetAllAsync(
                predicate: x => customerGroupCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
             .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        var existingCustomerGroupCodes = existingCustomerGroups
            .Keys
            .ToHashSet();

        var newCustomerGroupModels = models.Values
            .Where(x => !existingCustomerGroupCodes.Contains(x.Code))
            .ToDictionary(
                x => x.Code,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        Dictionary<string, CustomerGroup> newCustomerGroups = [];
        foreach (var model in newCustomerGroupModels.Values)
        {
            var customerGroup = CustomerGroupMapper.ToEntity(model);
            if (!customerGroup)
                return Operation.Error(customerGroup.Error);
            newCustomerGroups[customerGroup.Result.Code.Value] = customerGroup.Result;
        }

        var editResult = Edit(models, existingCustomerGroups);
        if (!editResult)
            return Operation.Error(editResult.Error);

        var customerGroups = newCustomerGroups.Values
            .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        foreach (var customerGroup in existingCustomerGroups)
        {
            customerGroups[customerGroup.Key] = customerGroup.Value;
        }

        var setParents = await SetParents(models, customerGroups);
        if (!setParents)
            return Operation.Error(setParents.Error);

        if (newCustomerGroups.Any())
        {
            await _unitOfWork.GetRepository<CustomerGroup>().InsertAsync(newCustomerGroups.Values);
        }

        if (existingCustomerGroupCodes.Any())
        {
            _unitOfWork.GetRepository<CustomerGroup>().Update(existingCustomerGroups.Values);
        }

        return true;
    }

    private async Task<Operation<bool, string>> SetParents(Dictionary<string, CustomerGroupDto> models, Dictionary<string, CustomerGroup> customerGroups)
    {
        var parentGroupCodes = models.Values
            .Where(x => x.ParentGroup is not null && !string.IsNullOrWhiteSpace(x.ParentGroup.Code))
            .Select(x => x.ParentGroup!.Code)
            .Distinct()
            .Select(CodeValue.Create);

        if (parentGroupCodes.Any(x => !x.Ok))
            return Operation.Error(parentGroupCodes.FirstOrDefault(x => !x.Ok).Error);

        var parents = (await _unitOfWork
            .GetRepository<CustomerGroup>()
            .GetAllAsync(
                predicate: x => parentGroupCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
             .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        var newParrents = customerGroups.Values
            .Where(x =>
                parentGroupCodes.Select(x => x.Result).Contains(x.Code)
                && !parents.Values.Select(x => x.Code).Contains(x.Code))
            .DistinctBy(x=>x.Code);

        foreach (var customerGroup in newParrents)
        {
            parents[customerGroup.Code.Value] = customerGroup;
        }

        foreach (var customerGroup in customerGroups.Values)
        {
            if (!models.TryGetValue(customerGroup.Code.Value, out var model))
                return Operation.Error("Customer group not found!");
            if (model is null)
                return Operation.Error($"Model {typeof(CustomerGroupDto).Name} not found!");

            if (model.ParentGroup is null || string.IsNullOrWhiteSpace(model.ParentGroup.Code))
            {
                customerGroup.SetParent(null);
            }
            else
            {
                if (!parents.TryGetValue(model.ParentGroup.Code, out var parent))
                    return Operation.Error($"Parent group '{model.ParentGroup!.Code}' not found for customer group '{customerGroup.Code.Value}'!");
                customerGroup.SetParent(parent);
            }
        }
        return true;
    }

    private Operation<bool, string> Edit(Dictionary<string, CustomerGroupDto> models, Dictionary<string, CustomerGroup> entities)
    {
        foreach (var entity in entities.Values)
        {
            if (!models.TryGetValue(entity.Code.Value, out var model))
                return Operation.Error($"Model {typeof(CustomerGroupDto).Name} not found!");

            var editResult = entity.EditEntity(model);
            if (!editResult.Ok)
                return Operation.Error(editResult.Error);
        }
        return true;
    }

    public Task<Operation<bool, string>> DeleteAsync(DateTime dateStart, DateTime dateEnd)
    {
        throw new NotImplementedException();
    }
}
