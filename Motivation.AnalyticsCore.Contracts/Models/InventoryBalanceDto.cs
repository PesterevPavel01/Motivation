namespace Motivation.AnalyticsCore.Contracts.Models;

public class InventoryBalanceDto
{
    public decimal TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    public required DateTime AsOfDate { get; set; }
    public required WarehouseDto Warehouse { get; set; }
}