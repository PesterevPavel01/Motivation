using Microsoft.Extensions.Logging;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Domain.Enum;
using Motivation.KpiCalculator.Core;
using Motivation.KpiCalculator.Core.Enums;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiSalesCalculator;
public sealed class SalesKpiCalculator : BaseKpiCalculator
{
    private readonly IEnumerable<ICalculationStrategy> _strategies;
    private readonly ILogger<SalesKpiCalculator> _logger;

    public SalesKpiCalculator(
        IKpiQueryBuilder queryBuilder,
        IEnumerable<ICalculationStrategy> strategies,
        ILogger<SalesKpiCalculator> logger)
        : base(queryBuilder, logger)
    {
        _strategies = strategies;
        _logger = logger;
    }

    public override KpiType KpiType => KpiType.Sales;

    public override ICalculationStrategy? GetStrategy(KpiCalculationTask task)
    {
        var StrategyType = task.MeasurementUnit.Title switch
        {
            "шт." => CalculationStrategyType.SalesQuantity,
            _ => CalculationStrategyType.SalesAmount
        };

        var strategy = _strategies.FirstOrDefault(x => x.StrategyType == StrategyType);
        if(strategy is null)
        {
            _logger.LogError($"Strategy for {CalculationStrategyType.SalesAmount.ToString()} not found!");
        }
        return strategy;
    }
}