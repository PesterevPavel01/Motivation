using Motivation.AnalyticsCore.Contracts.Models;

namespace Motivation.BasketComplexityKpiCalculator.Models;

public sealed class ComplexityCustomerProductItem()
{
    public int NumberParam { get; set; }
    public decimal Quantity { get; set; }
    public decimal Amount { get; set; }
    public required CustomerDto Customer { get; set; }
    public required ProductDto Product { get; set; }
    public required string TransactionCode { get; set; }

    public bool HasSuccess => Amount > 0;

};
