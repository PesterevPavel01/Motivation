using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto;
using Motivation.KpiCalculator.Core.Enums;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.ReceivablesKpiCalculator.Strategies;

public class AmountReceivableCalculationStrategy : ICalculationStrategy<ReceivableDto>
{
    public CalculationStrategyType StrategyType => CalculationStrategyType.ReceivableAmount;
    public Operation<decimal, string> Calculate(IEnumerable<ReceivableDto> transactions, MeasurementUnitDto measurementUnit)
    {
        return (-1) * transactions
            .Sum(x => x.Amount);
    }
}
