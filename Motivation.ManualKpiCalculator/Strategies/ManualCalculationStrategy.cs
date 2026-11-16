using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto;
using Motivation.Contracts.Models;
using Motivation.KpiCalculator.Core.Enums;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.ManualKpiCalculator.Strategies;

public class ManualCalculationStrategy : ICalculationStrategy<ManualResultDto>
{
    public CalculationStrategyType StrategyType => CalculationStrategyType.Manual;
    public Operation<decimal, string> Calculate(IEnumerable<ManualResultDto> results, MeasurementUnitDto measurementUnit)
    {
        return results
            .Sum(x => x.Value);
    }
}
