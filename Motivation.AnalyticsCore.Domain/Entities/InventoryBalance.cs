using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Domain.Entities;

public class InventoryBalance : Auditable
{
    protected InventoryBalance(Guid id, QuantityValue totalQuantity, AmountValue? totalAmount, DateTime asOfDate)
    {
        Id = id;
        TotalQuantity = totalQuantity;
        TotalAmount = totalAmount;
        AsOfDate = asOfDate;
    }
    public Guid Id { get; private set; }
    public QuantityValue TotalQuantity { get; private set; }
    public AmountValue? TotalAmount { get; private set; }
    public DateTime AsOfDate { get; private set; }

    public Warehouse? Warehouse { get; private set; }
    public Product Product { get; set; } = null!;

    public static Operation<InventoryBalance, string> Create(Warehouse? warehouse, Product product, QuantityValue totalQuantity, AmountValue? totalAmount, DateTime asOfDate)
    {
        if (product is null)
            return Operation.Error("Product is null!");
        if (totalQuantity is null)
            return Operation.Error("TotalQuantity is null!");

        var inventoryBalance = new InventoryBalance(Guid.CreateVersion7(), totalQuantity, totalAmount, asOfDate);

        var setProduct = inventoryBalance.SetProduct(product);
        if (!setProduct.Ok)
            return Operation.Error(setProduct.Error);

        inventoryBalance.SetWarehouse(warehouse);
        
        return inventoryBalance;
    }

    protected void SetWarehouse(Warehouse? warehouse)
    {
        Warehouse = warehouse;
    }
    protected Operation<bool, string> SetProduct(Product product)
    {
        if(product is null)
            return Operation.Error("Product is null!");
        Product = product;
        
        return true;
    }
}
