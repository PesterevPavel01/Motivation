using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Domain.Entities;

public class Product : Entity
{
    private readonly List<WarehouseTransaction> _warehouseTransactions = [];
    private readonly List<CustomerTransactionProduct> _customerTransactionProducts = [];
    private readonly List<InventoryBalance> _inventoryBalances = [];

    protected Product(Guid id, CodeValue code, TitleValue title, PurchaserValue? purchaser, StatusValue? status) : base(id, code)
    {
        Title = title;
        Purchaser = purchaser;
        Status = status;
    }

    public TitleValue Title { get; set; } = null!;
    public PurchaserValue? Purchaser { get; set; }
    public StatusValue? Status { get; set; }
    public Warehouse? Warehouse { get; set; }
    public ProductGroup? ProductGroup { get; set; }

    public IReadOnlyCollection<CustomerTransactionProduct> CustomerTransactionProducts => _customerTransactionProducts.AsReadOnly();
    public IReadOnlyCollection<WarehouseTransaction> WarehouseTransactions => _warehouseTransactions.AsReadOnly();
    public IReadOnlyCollection<InventoryBalance> InventoryBalances => _inventoryBalances.AsReadOnly();

    public static Operation<Product, string> Create(
        TitleValue title, 
        CodeValue code, 
        Warehouse? warehouse, 
        PurchaserValue? purchaser, 
        ProductGroup? productGroup,
        StatusValue? status)
    {
        if (title is null)
            return Operation.Error("Title is null!");
        if (code is null)
            return Operation.Error("Code is null!");
        
        var product = new Product(Guid.CreateVersion7(), code, title, purchaser, status);
        product.SetWarehouse(warehouse);
        product.SetProductGroup(productGroup);
        
        return product;
    }

    protected void SetWarehouse(Warehouse? warehouse)
    {
        Warehouse = warehouse;
    }
    protected void SetProductGroup(ProductGroup? productGroup)
    {
        ProductGroup = productGroup;
    }

    public Operation<Product, string> Edit(
        TitleValue title,
        Warehouse? warehouse,
        PurchaserValue? purchaser,
        ProductGroup? productGroup,
        StatusValue? status)
    {
        if (title is null)
            return Operation.Error("Title is null!");

        Title = title;
        Purchaser = purchaser;
        Status = status;
        SetWarehouse(warehouse);
        SetProductGroup(productGroup);

        return this;
    }
}
