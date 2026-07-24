using Motivation.BasketComplexityKpiCalculator.Models;

namespace Motivation.KpiComplexityCalculator.Models;

public class ComplexitySettings
{
    public required int MinUniqueGroups { get; set; }
    public required decimal MinRatio { get; set; }
    public bool IncludeZeroAmountProducts { get; set; }
    public List<ComplexityGroup> Groups { get; set; } = [];
    public List<CorrectionCoefficient> CorrectionCoefficients { get; set; } = [];
    public List<int> ExcludedNumberParams { get; internal set; } = [];
}
