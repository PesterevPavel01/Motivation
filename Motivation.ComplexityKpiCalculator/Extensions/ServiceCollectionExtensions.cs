using Microsoft.Extensions.DependencyInjection;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.BasketComplexityKpiCalculator;
using Motivation.BasketComplexityKpiCalculator.Mappers;
using Motivation.BasketComplexityKpiCalculator.Interfaces;
using Motivation.BasketComplexityKpiCalculator.Strategies;
using Motivation.BasketComplexityKpiCalculator.Validators;
using Motivation.Contracts.Interfaces;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiComplexityCalculator.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddComplexityKpiCalculator(this IServiceCollection source)
    {
        source.AddTransient<ICalculator, ComplexityKpiCalculator>();
        source.AddTransient<ICalculationStrategy, ComplexityCalculationStrategy>();
        source.AddScoped<IComplexityCalculationConfigurations, ComplexityCalculationConfigurations>();
        source.AddScoped<IValidator<IEnumerable<CustomerTransactionDto>>, ManagerValidator>();
        source.AddScoped<ComplexityItemsMapper>();
        source.AddScoped<IComplexityCalculateProcessor, ComplexityCalculateProcessor>();
    }
}
