using Microsoft.Extensions.Logging;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Domain.Enum;
using Motivation.KpiCalculator.Core;
using Motivation.KpiCalculator.Core.Enums;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.ReceivablesKpiCalculator;

public class ReceivablesKpiCalculator : BaseKpiCalculator<ReceivableDto>
{
    private readonly IEnumerable<ICalculationStrategy<ReceivableDto>> _strategies;
    private readonly ILogger<BaseKpiCalculator<ReceivableDto>> _logger;

    public ReceivablesKpiCalculator(
        IKpiQueryBuilder<ReceivableDto> queryBuilder,
        IEnumerable<ICalculationStrategy<ReceivableDto>> strategies,
        ILogger<BaseKpiCalculator<ReceivableDto>> logger)
        : base(queryBuilder, logger)
    {
        _strategies = strategies;
        _logger = logger;
    }
    public override KpiType KpiType => KpiType.Receivables;

    public override ICalculationStrategy<ReceivableDto>? GetStrategy(KpiCalculationTask task)
    {
        var strategy = _strategies.FirstOrDefault(x => x.StrategyType == CalculationStrategyType.ReceivableAmount);
        if (strategy is null)
        {
            _logger.LogError($"Strategy for {CalculationStrategyType.ReceivableAmount.ToString()} not found!");
        }
        return strategy;
    }
}
