using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto;

namespace Motivation.KpiSalesCalculator.Interfaces;

public interface ICalculationStrategy
{
    decimal Calculate(IEnumerable<CustomerTransactionDto> transactions, MeasurementUnitDto measurementUnit);
}

