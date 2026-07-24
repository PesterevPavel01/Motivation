using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.BasketComplexityKpiCalculator.Models;

namespace Motivation.BasketComplexityKpiCalculator.Interfaces;

public interface IComplexityCalculateProcessor
{
    decimal Process(List<ComplexityCustomerProductItem> items, IComplexityCalculationConfigurations complexityCalculationConfigurations);
}
