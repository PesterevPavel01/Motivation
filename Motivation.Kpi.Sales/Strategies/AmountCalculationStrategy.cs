using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.Contracts.Dto;
using Motivation.KpiSalesCalculator.Interfaces;

namespace Motivation.KpiSalesCalculator.Strategies;

public class AmountCalculationStrategy : ICalculationStrategy
{
    public decimal Calculate(IEnumerable<CustomerTransactionDto> transactions, MeasurementUnitDto measurementUnit)
    {
        return transactions
            .SelectMany(x => x.TransactionProducts)
            .Sum(ctp => ctp.Amount);
    }
}
