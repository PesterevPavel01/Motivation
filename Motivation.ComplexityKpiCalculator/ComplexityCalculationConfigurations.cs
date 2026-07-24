using Microsoft.Extensions.Options;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.BasketComplexityKpiCalculator.Interfaces;
using Motivation.KpiCalculator.Core.Interfaces;
using Motivation.KpiComplexityCalculator.Models;

namespace Motivation.BasketComplexityKpiCalculator;

public sealed class ComplexityCalculationConfigurations : IComplexityCalculationConfigurations
{
    private readonly ComplexitySettings _settings;
    private readonly IHierarchyService _hierarchyService;
    public ComplexityCalculationConfigurations(
        IOptions<ComplexitySettings> options,
        IHierarchyService hierarchyService)
    {
        _settings = options.Value;
        _hierarchyService = hierarchyService;
    }

    public int MinUniqueGroups => _settings.MinUniqueGroups;
    public decimal MinRatio => _settings.MinRatio;
    public List<int> ExcludedNumberParams => _settings.ExcludedNumberParams;
    public bool IncludeZeroAmountProducts => _settings.IncludeZeroAmountProducts;
    public Dictionary<string, int> GroupNumberMap =>
        _settings.Groups.ToDictionary(
            g => g.ProductCode,
            g => g.NumberParam,
            StringComparer.OrdinalIgnoreCase);

    public decimal GetCorrectionCoefficientByProduct(ProductDto product)
    {
        if (product is null)
            return 0;

        var hierarchyCodes = _hierarchyService.GetProductGroupHierarchy(product);

        var coefficients = _settings.CorrectionCoefficients.ToDictionary(
            g => g.ProductCode,
            g => g.Value,
            StringComparer.OrdinalIgnoreCase);

        foreach (var code in hierarchyCodes)
        {
            if (coefficients.TryGetValue(code, out var coefficient))
                return coefficient;
        }

        return 1.0m;
    }

    public int? FindNumberParamForProduct(ProductDto product)
    {
        if (product?.Code is null)
            return null;

        var hierarchyCodes = _hierarchyService.GetProductGroupHierarchy(product);

        foreach (var code in hierarchyCodes)
        {
            if (GroupNumberMap.TryGetValue(code, out var numberParam))
                return numberParam;
        }

        return null;
    }
}
