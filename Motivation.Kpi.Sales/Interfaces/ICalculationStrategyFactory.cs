using Motivation.Contracts.Dto;

namespace Motivation.KpiSalesCalculator.Interfaces;

public interface ICalculationStrategyFactory
{
    ICalculationStrategy GetStrategy(MeasurementUnitDto measurementUnit);
}
