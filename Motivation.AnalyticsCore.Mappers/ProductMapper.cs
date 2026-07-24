using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Mappers;

public static class ProductMapper
{
    public static ProductDto ToDto(this Product source) 
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        return new ProductDto()
        {
            Code = source.Code.Value,
            Title = source.Title.Value,
            Purchaser = source.Purchaser?.Value,
            Status = source.Status?.Value,
            Supplier = source.Supplier?.Value,
            Warehouse = source.Warehouse is null ?
                null
                : source.Warehouse.ToDto(),

            ProductGroup = source.ProductGroup is null ?
                null
                : source.ProductGroup.ToDto(),
        };
    }

    public static Operation<Product, string> EditEntity(this Product product, ProductDto model, Dictionary<string, Warehouse> existingWarehouses, Dictionary<string, ProductGroup> existingProductGroups)
    {
        if (model is null)
            return Operation.Error("Product DTO is null.");

        var titleResult = TitleValue.Create(model.Title);
        if (!titleResult.Ok)
            return Operation.Error(titleResult.Error!);

        var codeResult = CodeValue.Create(model.Code);
        if (!codeResult.Ok)
            return Operation.Error(codeResult.Error);

        SupplierValue? supplier = null;
        if (!string.IsNullOrWhiteSpace(model.Supplier))
        {
            var supplierResult = SupplierValue.Create(model.Supplier);
            if (!supplierResult.Ok)
                return Operation.Error(supplierResult.Error);
            supplier = supplierResult.Result;
        }

        PurchaserValue? purchaser = null;
        if (!string.IsNullOrWhiteSpace(model.Purchaser))
        {
            var purchaserResult = PurchaserValue.Create(model.Purchaser);
            if (!purchaserResult.Ok)
                return Operation.Error(purchaserResult.Error);
            purchaser = purchaserResult.Result;
        }

        StatusValue? status = null;
        if (!string.IsNullOrWhiteSpace(model.Status))
        {
            var statusResult = StatusValue.Create(model.Status);
            if (!statusResult.Ok)
                return Operation.Error(statusResult.Error);
            status = statusResult.Result;
        }

        Warehouse? warehouse = null;
        if (model.Warehouse is not null && !string.IsNullOrWhiteSpace(model.Warehouse.Code))
        {
            var getWarehouse = GetWarehouse(model.Warehouse, existingWarehouses);
            if (!getWarehouse)
                return Operation.Error(getWarehouse.Error);
            warehouse = getWarehouse.Result;
        }

        var productGroup = GetProductGroup(model.ProductGroup, existingProductGroups);

        return product.Edit(
            titleResult.Result,
            warehouse,
            supplier,
            purchaser,
            productGroup.Result,
            status
        );
    }

    public static Operation<Product, string> ToEntity(ProductDto model, Dictionary<string, Warehouse> existingWarehouses, Dictionary<string, ProductGroup> existingProductGroups)
    {
        if (model is null)
            return Operation.Error("Product DTO is null.");

        var titleResult = TitleValue.Create(model.Title);
        if (!titleResult.Ok)
            return Operation.Error(titleResult.Error!);

        var codeResult = CodeValue.Create(model.Code);
        if (!codeResult.Ok)
            return Operation.Error(codeResult.Error);

        SupplierValue? supplier = null;
        if (!string.IsNullOrWhiteSpace(model.Supplier))
        {
            var supplierResult = SupplierValue.Create(model.Supplier);
            if (!supplierResult.Ok)
                return Operation.Error(supplierResult.Error);
            supplier = supplierResult.Result;
        }

        PurchaserValue? purchaser = null;
        if (!string.IsNullOrWhiteSpace(model.Purchaser))
        {
            var purchaserResult = PurchaserValue.Create(model.Purchaser);
            if (!purchaserResult.Ok)
                return Operation.Error(purchaserResult.Error);
            purchaser = purchaserResult.Result;
        }

        StatusValue? status = null;
        if (!string.IsNullOrWhiteSpace(model.Status))
        {
            var statusResult = StatusValue.Create(model.Status);
            if (!statusResult.Ok)
                return Operation.Error(statusResult.Error);
            status = statusResult.Result;
        }

        Warehouse? warehouse = null;

        if (model.Warehouse is not null && !string.IsNullOrWhiteSpace(model.Warehouse.Code))
        {
            var getWarehouse = GetWarehouse(model.Warehouse, existingWarehouses);
            if (!getWarehouse)
                return Operation.Error(getWarehouse.Error);
            warehouse = getWarehouse.Result;
        }

        var productGroup = GetProductGroup(model.ProductGroup, existingProductGroups);

        return Product.Create(
            titleResult.Result,
            codeResult.Result,
            warehouse,
            supplier,
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
