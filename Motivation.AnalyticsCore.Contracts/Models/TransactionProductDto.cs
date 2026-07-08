namespace Motivation.AnalyticsCore.Contracts.Models;

public sealed class TransactionProductDto
{
    public decimal Quantity { get; set; }
    public decimal Amount { get; set; }
    public required ProductDto Product { get; set; }
}