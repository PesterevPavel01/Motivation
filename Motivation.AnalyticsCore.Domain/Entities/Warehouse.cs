using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Domain.Entities;

public class Warehouse : SimpleEntity
{
    private readonly List<InventoryBalance> _inventoryBalances = [];
    private readonly List<WarehouseTransaction> _warehouseTransactions = [];
    private readonly List<Product> _products = [];
    protected Warehouse(Guid id, CodeValue code, TitleValue title) : base(title, code, id)
    {
    }

    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();
    public IReadOnlyCollection<InventoryBalance> InventoryBalances => _inventoryBalances.AsReadOnly();
    public IReadOnlyCollection<WarehouseTransaction> WarehouseTransactions => _warehouseTransactions.AsReadOnly();

    public static Operation<Warehouse, string> Create(TitleValue title, CodeValue code)
    {
        if (title is null)
            return Operation.Error("Title is null!");
        if (code is null)
            return Operation.Error("Code is null!");

        return new Warehouse(Guid.CreateVersion7(), code, title);
    }
    public Operation<bool, string> AddTransaction(WarehouseTransaction warehouseTransactions)
    {
        if (warehouseTransactions is null)
            return Operation.Error("WarehouseTransaction is null!");

        if (_warehouseTransactions.Any(x => x.Code == warehouseTransactions.Code))
            return Operation.Error($"Transaction with code '{warehouseTransactions.Code}' already exists!");
        _warehouseTransactions.Add(warehouseTransactions);
        return true;
    }
}
