using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;
using Motivation.AnalyticsCore.Mappers;

namespace Motivation.AnalyticsCore.Application.Services;

public sealed class InventoryBalanceService : IEntityServices<InventoryBalanceDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public InventoryBalanceService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Operation<bool, string>> CreateOrEditAsync(Dictionary<string, InventoryBalanceDto> models, DateTime dateStart, DateTime dateEnd, CancellationToken cancellationToken)
    {
        var productCodes = models.Values
            .Select(x => x.ProductCode)
            .Distinct()
            .Select(CodeValue.Create);
        if (productCodes.Any(x => !x.Ok))
            return Operation.Error(productCodes.FirstOrDefault(x => !x.Ok).Error);

        var warehouseCodes = models.Values
            .Select(x => x.WarehouseCode)
            .Distinct()
            .Select(CodeValue.Create);
        if (productCodes.Any(x => !x.Ok))
            return Operation.Error(productCodes.FirstOrDefault(x => !x.Ok).Error);

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

        var warehouses = (await _unitOfWork
            .GetRepository<Warehouse>()
            .GetAllAsync(
                predicate: x => warehouseCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
            .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        var create = Create(models, products, warehouses);
        if (!create.Ok)
        {
            return Operation.Error(create.Error);
        }
        return true;
    }

    public async Task<Operation<bool, string>> DeleteAsync(DateTime dateStart, DateTime dateEnd)
    {
        var inventoryBalances = await _unitOfWork
            .GetRepository<InventoryBalance>()
            .GetAllAsync(
                predicate: x => x.AsOfDate >= dateStart && x.AsOfDate <= dateEnd,
                trackingType: TrackingType.Tracking,
                include: null);

        _unitOfWork.GetRepository<InventoryBalance>().Delete(inventoryBalances);

        return true;
    }

    private Operation<bool, string> Create(
        Dictionary<string, InventoryBalanceDto> models,
        Dictionary<string, Product> products,
        Dictionary<string, Warehouse> warehouses)
    {
        List<InventoryBalance> inventoryBalances = [];
        foreach (var model in models.Values)
        {
            if (!products.TryGetValue(model.ProductCode, out var product))
                return Operation.Error("Product not found!");
            if (product is null)
                return Operation.Error("Product is null!");

            if (!warehouses.TryGetValue(model.WarehouseCode, out var warehouse))
                return Operation.Error("Warehouse not found!");
            if (warehouse is null)
                return Operation.Error("Warehouse is null!");

            var inventoryBalance = InventoryBalanceMapper.ToEntity(model, product, warehouse);
            if (!inventoryBalance.Ok)
                return Operation.Error(inventoryBalance.Error);

            inventoryBalances.Add(inventoryBalance.Result);
        }

        if (inventoryBalances.Any())
        {
            _unitOfWork.GetRepository<InventoryBalance>()
                .Insert(inventoryBalances);
        }
        return true;
    }
}