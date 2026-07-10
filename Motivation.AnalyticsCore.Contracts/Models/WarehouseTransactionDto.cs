namespace Motivation.AnalyticsCore.Contracts.Models;

public sealed class WarehouseTransactionDto
{
    public required string Code { get; set; }
    public decimal Quantity { get; set; }
    public decimal Amount { get; set; }
    public required DateTime AsOfDate { get; set; }
    public required string Type { get; set; }
    public required string WarehouseCode { get; set; }
    public required string ProductCode { get; set; }
}