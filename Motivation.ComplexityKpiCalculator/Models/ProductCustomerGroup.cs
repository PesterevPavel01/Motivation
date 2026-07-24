using Motivation.AnalyticsCore.Contracts.Models;

namespace Motivation.BasketComplexityKpiCalculator.Models;

public record ProductCustomerGroup
{
    public string ProductCode { get; init; } = string.Empty;
    public string CustomerCode { get; init; } = string.Empty;
    public int NumberParam { get; init; }
    public decimal TotalQuantity { get; init; }
    public decimal TotalAmount { get; init; }
    public CustomerDto Customer { get; init; } = null!;
    public ProductDto Product { get; init; } = null!;
}