using Motivation.AnalyticsCore.Contracts.Models;

namespace Motivation.BasketComplexityKpiCalculator.Interfaces;

public interface IComplexityCalculationConfigurations
{
    decimal GetCorrectionCoefficientByProduct(ProductDto product);
    Dictionary<string, int> GroupNumberMap { get; }
    int MinUniqueGroups {  get; }
    decimal MinRatio {  get; }
    List<int> ExcludedNumberParams { get; }
    bool IncludeZeroAmountProducts { get; }

    int? FindNumberParamForProduct(ProductDto product);

}
