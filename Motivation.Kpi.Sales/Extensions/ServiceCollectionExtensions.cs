using Microsoft.Extensions.DependencyInjection;
using Motivation.Contracts.Interfaces;
using Motivation.KpiSalesCalculator.Interfaces;
using Motivation.KpiSalesCalculator.Providers;
using Motivation.KpiSalesCalculator.Services;
using Motivation.KpiSalesCalculator.Specifications;
using Motivation.KpiSalesCalculator.Strategies;

namespace Motivation.KpiSalesCalculator.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddSalesKpiCalculator(this IServiceCollection source)
    {
        source.AddScoped(typeof(ISpecificationBuilder<>), typeof(SpecificationBuilder<>));

        source.AddScoped<ICalculationStrategyFactory, CalculationStrategyFactory>();
        source.AddScoped<AmountCalculationStrategy>();
        source.AddScoped<QuantityCalculationStrategy>();

        source.AddScoped<IHierarchyService, HierarchyService>();
        source.AddScoped<ITransactionDataProvider, TransactionDataProvider>();
        source.AddTransient<IKpiQueryBuilder, KpiQueryBuilder>();
        source.AddTransient<ICalculator, SalesKpiCalculator>();
    }
}
