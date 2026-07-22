using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto;
using Motivation.KpiSalesCalculator.Interfaces;

namespace Motivation.KpiSalesCalculator.Strategies;

public class QuantityCalculationStrategy : ICalculationStrategy
{
    public decimal Calculate(IEnumerable<CustomerTransactionDto> transactions, MeasurementUnitDto measurementUnit)
    {
        return transactions
            .SelectMany(ct => ct.TransactionProducts)
            .Sum(ctp => ctp.Quantity);
    }
}
