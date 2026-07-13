using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Services;

public sealed class ProductGroupService : IEntityServices<ProductGroupDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public ProductGroupService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Operation<bool, string>> CreateOrEditAsync(Dictionary<string, ProductGroupDto> models, DateTime dateStart, DateTime dateEnd, CancellationToken cancellationToken)
    {
        var productGroupCodes =
            models.Values
            .Select(x => CodeValue.Create(x.Code));

        var existingProductGroups = (await _unitOfWork
            .GetRepository<ProductGroup>()
            .GetAllAsync(
                predicate: x => productGroupCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
             .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        var existingProductGroupCodes = existingProductGroups
            .Keys
            .ToHashSet();

        var newProductGroupModels = models.Values
            .Where(x => !existingProductGroupCodes.Contains(x.Code))
            .ToDictionary(
                x => x.Code,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        Dictionary<string, ProductGroup> newProductGroups = [];
        foreach (var model in newProductGroupModels.Values)
        {
            var productGroup = ProductGroupMapper.ToEntity(model);
            if (!productGroup)
                return Operation.Error(productGroup.Error);
            newProductGroups[productGroup.Result.Code.Value] = productGroup.Result;
        }

        var editResult = Edit(models, existingProductGroups);
        if (!editResult)
            return Operation.Error(editResult.Error);

        var productGroups = newProductGroups.Values
            .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        foreach (var productGroup in existingProductGroups)
        {
            productGroups[productGroup.Key] = productGroup.Value;
        }

        var setParents = await SetParents(models, productGroups);
        if (!setParents)
            return Operation.Error(setParents.Error);

        if (newProductGroups.Any())
        {
            await _unitOfWork.GetRepository<ProductGroup>().InsertAsync(newProductGroups.Values);
        }

        if (existingProductGroupCodes.Any())
        {
            _unitOfWork.GetRepository<ProductGroup>().Update(existingProductGroups.Values);
        }

        return true;
    }
    private async Task<Operation<bool, string>> SetParents(Dictionary<string, ProductGroupDto> models, Dictionary<string, ProductGroup> productGroups)
    {
        var parentGroupCodes = models.Values
            .Where(x => x.ParentGroup is not null && !string.IsNullOrWhiteSpace(x.ParentGroup.Code))
            .Select(x => x.ParentGroup!.Code)
            .Distinct()
            .Select(CodeValue.Create);

        if (parentGroupCodes.Any(x => !x.Ok))
            return Operation.Error(parentGroupCodes.FirstOrDefault(x => !x.Ok).Error);

        var parents = (await _unitOfWork
            .GetRepository<ProductGroup>()
            .GetAllAsync(
                predicate: x => parentGroupCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
             .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        var newParrents = productGroups.Values
            .Where(x =>
                parentGroupCodes.Select(x => x.Result).Contains(x.Code)
                && !parents.Values.Select(x => x.Code).Contains(x.Code))
            .DistinctBy(x => x.Code);

        foreach (var productGroup in newParrents)
        {
            parents[productGroup.Code.Value] = productGroup;
        }

        foreach (var productGroup in productGroups.Values)
        {
            if (!models.TryGetValue(productGroup.Code.Value, out var model))
                return Operation.Error("Product group not found!");
            if (model is null)
                return Operation.Error($"Model {typeof(ProductGroupDto).Name} not found!");
            
            if (model.ParentGroup is null || string.IsNullOrWhiteSpace(model.ParentGroup.Code))
            {
                productGroup.SetParent(null);
            }
            else
            {
                if (!parents.TryGetValue(model.ParentGroup.Code, out var parent))
                    return Operation.Error($"Parent group '{model.ParentGroup!.Code}' not found for product group '{productGroup.Code.Value}'!");
                productGroup.SetParent(parent);
            }
        }
        return true;
    }

    private Operation<bool, string> Edit(Dictionary<string, ProductGroupDto> models, Dictionary<string, ProductGroup> entities)
    {
        foreach (var entity in entities.Values)
        {
            if (!models.TryGetValue(entity.Code.Value, out var model))
                return Operation.Error($"Model {typeof(ProductGroupDto).Name} not found!");

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
