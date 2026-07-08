using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Mappers;

public static class ProductMapper
{
    public static Operation<Product, string> EditEntity(this Product product, ProductDto dto, Dictionary<string, Warehouse> existingWarehouses, Dictionary<string, ProductGroup> existingProductGroups)
    {
        if (dto is null)
            return Operation.Error("Product DTO is null.");

        var titleResult = TitleValue.Create(dto.Title);
        if (!titleResult.Ok)
            return Operation.Error(titleResult.Error!);

        var codeResult = CodeValue.Create(dto.Code);
        if (!codeResult.Ok)
            return Operation.Error(codeResult.Error);

        PurchaserValue? purchaser = null;
        if (!string.IsNullOrWhiteSpace(dto.Purchaser))
        {
            var purchaserResult = PurchaserValue.Create(dto.Purchaser);
            if (!purchaserResult.Ok)
                return Operation.Error(purchaserResult.Error);
            purchaser = purchaserResult.Result;
        }

        StatusValue? status = null;
        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            var statusResult = StatusValue.Create(dto.Status);
            if (!statusResult.Ok)
                return Operation.Error(statusResult.Error);
            status = statusResult.Result;
        }

        Warehouse? warehouse = null;
        if (dto.Warehouse is not null && !string.IsNullOrWhiteSpace(dto.Warehouse.Code))
        {
            var getWarehouse = GetWarehouse(dto.Warehouse, existingWarehouses);
            if (!getWarehouse)
                return Operation.Error(getWarehouse.Error);
            warehouse = getWarehouse.Result;
        }

        var productGroup = GetProductGroup(dto.ProductGroup, existingProductGroups);

        return product.Edit(
            titleResult.Result,
            warehouse,
            purchaser,
            productGroup.Result,
            status
        );
    }

    public static Operation<Product, string> ToEntity(ProductDto dto, Dictionary<string, Warehouse> existingWarehouses, Dictionary<string, ProductGroup> existingProductGroups)
    {
        if (dto is null)
            return Operation.Error("Product DTO is null.");

        var titleResult = TitleValue.Create(dto.Title);
        if (!titleResult.Ok)
            return Operation.Error(titleResult.Error!);

        var codeResult = CodeValue.Create(dto.Code);
        if (!codeResult.Ok)
            return Operation.Error(codeResult.Error);

        PurchaserValue? purchaser = null;
        if (!string.IsNullOrWhiteSpace(dto.Purchaser))
        {
            var purchaserResult = PurchaserValue.Create(dto.Purchaser);
            if (!purchaserResult.Ok)
                return Operation.Error(purchaserResult.Error);
            purchaser = purchaserResult.Result;
        }

        StatusValue? status = null;
        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            var statusResult = StatusValue.Create(dto.Status);
            if (!statusResult.Ok)
                return Operation.Error(statusResult.Error);
            status = statusResult.Result;
        }

        Warehouse? warehouse = null;

        if (dto.Warehouse is not null && !string.IsNullOrWhiteSpace(dto.Warehouse.Code))
        {
            var getWarehouse = GetWarehouse(dto.Warehouse, existingWarehouses);
            if (!getWarehouse)
                return Operation.Error(getWarehouse.Error);
            warehouse = getWarehouse.Result;
        }

        var productGroup = GetProductGroup(dto.ProductGroup, existingProductGroups);

        return Product.Create(
            titleResult.Result,
            codeResult.Result,
            warehouse,
            purchaser,
            productGroup.Result,
            status
        );
    }

    private static Operation<Warehouse?, string> GetWarehouse(WarehouseDto? model, Dictionary<string, Warehouse> existingWarehouses) 
    {
        if (model is null)
            return null;

        var warehouseCode = CodeValue.Create(model.Code);
        if (!warehouseCode.Ok)
            return Operation.Error(warehouseCode.Error);

        var code = warehouseCode.Result.Value;
        if (existingWarehouses.TryGetValue(code, out var existingWarehouse))
            return existingWarehouse;
       
        var warehouseResult = WarehouseMapper.ToEntity(model);
        if (!warehouseResult.Ok)
            return Operation.Error(warehouseResult.Error);

        existingWarehouses[code] = warehouseResult.Result;

        return warehouseResult.Result;
    }

    private static Operation<ProductGroup?, string> GetProductGroup(ProductGroupDto? model, Dictionary<string, ProductGroup> existingProductGroups)
    {
        if (model is null)
            return null;

        var productGroupCode = CodeValue.Create(model.Code);
        if (!productGroupCode.Ok)
            return Operation.Error(productGroupCode.Error);

        var code = productGroupCode.Result.Value;

        if (existingProductGroups.TryGetValue(code, out var existingProductGroup))
            return existingProductGroup;

        var productGroupResult = ProductGroupMapper.ToEntity(model);
        if (!productGroupResult.Ok)
            return Operation.Error(productGroupResult.Error);

        existingProductGroups[code] = productGroupResult.Result;

        return productGroupResult.Result;
    }
}
