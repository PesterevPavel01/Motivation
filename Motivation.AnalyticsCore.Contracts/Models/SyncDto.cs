namespace Motivation.AnalyticsCore.Contracts.Models;

public class SyncDto
{
    public IEnumerable<CompanyDto> Companies { get; set; } = [];
    public IEnumerable<WarehouseDto> Warehouses { get; set; } = [];
    public IEnumerable<ProductDto> Products { get; set; } = [];
    public IEnumerable<ProductGroupDto> ProductGroups { get; set; } = [];
    public IEnumerable<CustomerDto> Customers { get; set; } = [];
    public IEnumerable<CustomerGroupDto> CustomerGroups { get; set; } = [];
    public IEnumerable<ReceivableDto> Receivables { get; set; } = [];
    public IEnumerable<CustomerTransactionDto> CustomerTransactions { get; set; } = [];
    public IEnumerable<InventoryBalanceDto> InventoryBalances { get; set; } = [];
    public IEnumerable<WarehouseTransactionDto> WarehouseTransactions { get; set; } = [];
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
}
