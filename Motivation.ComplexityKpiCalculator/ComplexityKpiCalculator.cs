using Microsoft.Extensions.Logging;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Domain.Enum;
using Motivation.KpiCalculator.Core;
using Motivation.KpiCalculator.Core.Enums;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiComplexityCalculator;

public sealed class ComplexityKpiCalculator : BaseKpiCalculator< CustomerTransactionDto>
{
    private readonly IEnumerable<ICalculationStrategy<CustomerTransactionDto>> _strategies;
    private readonly ILogger<ComplexityKpiCalculator> _logger;

    public ComplexityKpiCalculator(
        IKpiQueryBuilder<CustomerTransactionDto> queryBuilder,
        IEnumerable<ICalculationStrategy<CustomerTransactionDto>> strategies,
        ILogger<ComplexityKpiCalculator> logger)
        : base(queryBuilder, logger)
    {
        _strategies = strategies;
        _logger = logger;
    }

    public override KpiType KpiType => KpiType.Complexity;

    public override ICalculationStrategy<CustomerTransactionDto>? GetStrategy(KpiCalculationTask task)
    {
        var strategy = _strategies.FirstOrDefault(x => x.StrategyType == CalculationStrategyType.Complexity);
        if (strategy is null)
        {
            _logger.LogError($"Strategy for {CalculationStrategyType.Complexity.ToString()} not found!");
        }
        return strategy;
    }
}