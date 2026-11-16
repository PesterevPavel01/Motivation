using Microsoft.Extensions.Logging;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Contracts.Models;
using Motivation.Domain.Enum;
using Motivation.KpiCalculator.Core;
using Motivation.KpiCalculator.Core.Enums;
using Motivation.KpiCalculator.Core.Interfaces;
using Rebus.Bus;

namespace Motivation.ManualKpiCalculator;

public class ManualKpiCalculator : BaseKpiCalculator<ManualResultDto>
{
    private readonly IEnumerable<ICalculationStrategy<ManualResultDto>> _strategies;
    private readonly ILogger<BaseKpiCalculator<ManualResultDto>> _logger;

    public ManualKpiCalculator(
        IKpiQueryBuilder<ManualResultDto> queryBuilder,
        IEnumerable<ICalculationStrategy<ManualResultDto>> strategies,
        ILogger<BaseKpiCalculator<ManualResultDto>> logger,
        IBus bus)
        : base(queryBuilder, logger)
    {
        _strategies = strategies;
        _logger = logger;
    }
    public override KpiType KpiType => KpiType.Manual;

    public override ICalculationStrategy<ManualResultDto>? GetStrategy(KpiCalculationTask task)
    {
        var strategy = _strategies.FirstOrDefault(x => x.StrategyType == CalculationStrategyType.Manual);
        if (strategy is null)
        {
            _logger.LogError($"Strategy for {CalculationStrategyType.Manual.ToString()} not found!");
        }
        return strategy;
    }

    protected override async Task<KpiQueryResult<ManualResultDto>?> BuildQueryAsync(
        KpiCalculationTask task,
        CancellationToken cancellationToken)
    {
        if (task.ManualResult is null)
        {
            throw new ArgumentException("Manual result is null!");
        }

        return 
            new KpiQueryResult<ManualResultDto>
            {
                Transactions = [task.ManualResult]
            };
    }
}
