using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto;
using Motivation.KpiCalculator.Core.Enums;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiCalculator.Core.Strategies;

public class AmountCalculationStrategy : ICalculationStrategy<CustomerTransactionDto>
{
    public CalculationStrategyType StrategyType => CalculationStrategyType.SalesAmount;
    public Operation<decimal, string> Calculate(IEnumerable<CustomerTransactionDto> transactions, MeasurementUnitDto measurementUnit)
    {
        return transactions
            .SelectMany(x => x.TransactionProducts)
            .Sum(ctp => ctp.Amount);
    }
}
