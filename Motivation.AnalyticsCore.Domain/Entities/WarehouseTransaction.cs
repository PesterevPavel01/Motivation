using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;
using Motivation.AnalyticsCore.Domain.Enum;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Domain.Entities;

public class WarehouseTransaction : Entity
{
    protected WarehouseTransaction(Guid id, CodeValue code, WarehouseTransactionType type, QuantityValue? quantity, AmountValue? amount, DateTime asOfDate) : base(id, code)
    {
        Type = type;
        Quantity = quantity;
        Amount = amount;
        AsOfDate = asOfDate;
    }

    public QuantityValue? Quantity { get; protected set; }
    public AmountValue? Amount { get; protected set; }

    public DateTime AsOfDate { get; private set; }

    public WarehouseTransactionType Type { get; protected set; }

    public Product Product { get; protected set; } = null!;
    public Warehouse Warehouse { get; protected set; } = null!;

    public static Operation<WarehouseTransaction, string> Create(
        CodeValue code, 
        WarehouseTransactionType type, 
        QuantityValue? quantity, 
        AmountValue? amount, 
        DateTime asOfDate, 
        Product product,
        Warehouse warehouse)
    {
        if (code is null)
            return Operation.Error("Code is null!");

        var warehouseTransaction = new WarehouseTransaction(Guid.CreateVersion7(), code, type, quantity, amount, asOfDate);
        
        var setProduct = warehouseTransaction.SetProduct(product);
        if (!setProduct.Ok)
            return Operation.Error(setProduct.Error);
        var setWarehouse = warehouseTransaction.SetWarehouse(warehouse);
        if (!setWarehouse.Ok)
            return Operation.Error(setWarehouse.Error);

        return warehouseTransaction;
    }

    public Operation<bool, string> Edit(
        WarehouseTransactionType type, 
        QuantityValue? quantity, 
        AmountValue? amount, 
        DateTime asOfDate, 
        Product product,
        Warehouse warehouse)
    {
        Type = type;
        Quantity = quantity;
        Amount = amount;
        AsOfDate = asOfDate;

        var setProduct = SetProduct(product);
        if (!setProduct.Ok)
            return Operation.Error(setProduct.Error);

        var setWarehouse = SetWarehouse(warehouse);
        if (!setWarehouse.Ok)
            return Operation.Error(setWarehouse.Error);

        return true;
    }

    protected Operation<bool, string> SetProduct(Product product)
    {
        if (product is null)
            return Operation.Error("Product is null!");
        Product = product;

        return true;
    }

    protected Operation<bool, string> SetWarehouse(Warehouse warehouse)
    {
        if (warehouse is null)
            return Operation.Error("Warehouse is null!");
        Warehouse = warehouse;

        return true;
    }
}
