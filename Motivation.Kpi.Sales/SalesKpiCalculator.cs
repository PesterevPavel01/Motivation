using Microsoft.Extensions.Logging;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Domain.Enum;
using Motivation.KpiCalculator.Core;
using Motivation.KpiCalculator.Core.Enums;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiSalesCalculator;
public sealed class SalesKpiCalculator : BaseKpiCalculator<CustomerTransactionDto>
{
    private readonly IEnumerable<ICalculationStrategy<CustomerTransactionDto>> _strategies;
    private readonly ILogger<SalesKpiCalculator> _logger;

    public SalesKpiCalculator(
        IKpiQueryBuilder<CustomerTransactionDto> queryBuilder,
        IEnumerable<ICalculationStrategy<CustomerTransactionDto>> strategies,
        ILogger<SalesKpiCalculator> logger)
        : base(queryBuilder, logger)
    {
        _strategies = strategies;
        _logger = logger;
    }

    public override KpiType KpiType => KpiType.Sales;

    public override ICalculationStrategy<CustomerTransactionDto>? GetStrategy(KpiCalculationTask task)
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