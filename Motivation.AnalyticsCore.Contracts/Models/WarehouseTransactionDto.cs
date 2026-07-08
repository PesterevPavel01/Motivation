namespace Motivation.AnalyticsCore.Contracts.Models;

public sealed class WarehouseTransactionDto
{
    public decimal Quantity { get; set; }
    public decimal Amount { get; set; }
    public required DateTime AsOfDate { get; set; }
    public required string Type { get; set; }
    public ProductDto Product { get; set; } = null!;
}