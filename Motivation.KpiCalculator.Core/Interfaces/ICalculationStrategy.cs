using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto;
using Motivation.KpiCalculator.Core.Enums;

namespace Motivation.KpiCalculator.Core.Interfaces;

public interface ICalculationStrategy<TDto>
    where TDto : class
{
    CalculationStrategyType StrategyType { get; }
    Operation<decimal, string> Calculate(IEnumerable<TDto> transactions, MeasurementUnitDto measurementUnit);
}

