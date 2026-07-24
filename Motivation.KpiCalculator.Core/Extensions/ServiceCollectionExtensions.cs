using Microsoft.Extensions.DependencyInjection;
using Motivation.Contracts.Interfaces;
using Motivation.KpiCalculator.Core.Interfaces;
using Motivation.KpiCalculator.Core.Providers;
using Motivation.KpiCalculator.Core.Services;
using Motivation.KpiCalculator.Core.Specifications;
using Motivation.KpiCalculator.Core.Strategies;

namespace Motivation.KpiCalculator.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddKpiCalculator(this IServiceCollection source)
    {
        source.AddScoped(typeof(ISpecificationBuilder<>), typeof(SpecificationBuilder<>));
        source.AddScoped<ICalculationStrategy, AmountCalculationStrategy>();
        source.AddScoped<ICalculationStrategy, QuantityCalculationStrategy>();
        source.AddScoped<IHierarchyService, HierarchyService>();
        source.AddScoped<ITransactionDataProvider, TransactionDataProvider>();
        source.AddTransient<IKpiQueryBuilder, KpiQueryBuilder>();
    }
}
