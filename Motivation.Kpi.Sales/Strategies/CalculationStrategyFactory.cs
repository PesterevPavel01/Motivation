using Motivation.Contracts.Dto;
using Motivation.KpiSalesCalculator.Interfaces;

namespace Motivation.KpiSalesCalculator.Strategies;

public class CalculationStrategyFactory : ICalculationStrategyFactory
{
    private readonly Dictionary<string, ICalculationStrategy> _strategies = new()
    {
        ["Amount"] = new AmountCalculationStrategy(),
        ["Quantity"] = new QuantityCalculationStrategy()
    };

    public ICalculationStrategy GetStrategy(MeasurementUnitDto measurementUnit)
    {
        var key = measurementUnit.Title switch
        {
            "шт." => "Quantity",
            _ => "Amount"
        };

        return _strategies.GetValueOrDefault(key) ?? _strategies["Amount"];
    }
}
