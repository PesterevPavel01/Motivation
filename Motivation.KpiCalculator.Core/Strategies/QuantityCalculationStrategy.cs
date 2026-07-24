using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto;
using Motivation.KpiCalculator.Core.Enums;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiCalculator.Core.Strategies;

public class QuantityCalculationStrategy : ICalculationStrategy
{
    public CalculationStrategyType StrategyType => CalculationStrategyType.SalesQuantity;
    public Operation<decimal, string> Calculate(IEnumerable<CustomerTransactionDto> transactions, MeasurementUnitDto measurementUnit)
    {
        return transactions
            .SelectMany(ct => ct.TransactionProducts)
            .Sum(ctp => ctp.Quantity);
    }
}
