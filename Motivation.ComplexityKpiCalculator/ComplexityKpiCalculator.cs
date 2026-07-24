using Microsoft.Extensions.Logging;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Domain.Enum;
using Motivation.KpiCalculator.Core;
using Motivation.KpiCalculator.Core.Enums;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiComplexityCalculator;

public sealed class ComplexityKpiCalculator : BaseKpiCalculator
{
    private readonly IEnumerable<ICalculationStrategy> _strategies;
    private readonly ILogger<ComplexityKpiCalculator> _logger;

    public ComplexityKpiCalculator(
        IKpiQueryBuilder queryBuilder,
        IEnumerable<ICalculationStrategy> strategies,
        ILogger<ComplexityKpiCalculator> logger)
        : base(queryBuilder, logger)
    {
        _strategies = strategies;
        _logger = logger;
    }

    public override KpiType KpiType => KpiType.Complexity;

    public override ICalculationStrategy? GetStrategy(KpiCalculationTask task)
    {
        var strategy = _strategies.FirstOrDefault(x => x.StrategyType == CalculationStrategyType.Complexity);
        if (strategy is null)
        {
            _logger.LogError($"Strategy for {CalculationStrategyType.Complexity.ToString()} not found!");
        }
        return strategy;
    }
}