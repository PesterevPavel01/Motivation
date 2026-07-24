namespace Motivation.BasketComplexityKpiCalculator.Models;

public record ComplexityResult
{
    public string MasterCode { get; init; } = string.Empty;
    public int SumNumberParams { get; init; }
    public decimal FilteredAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal Ratio { get; init; }
    public bool IsSuccess { get; init; }
}
