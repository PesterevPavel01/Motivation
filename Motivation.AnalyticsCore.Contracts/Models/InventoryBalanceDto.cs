namespace Motivation.AnalyticsCore.Contracts.Models;

public class InventoryBalanceDto
{
    public decimal TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    public required DateTime AsOfDate { get; set; }
    public required string WarehouseCode { get; set; }
    public required string ProductCode { get; set; }
}