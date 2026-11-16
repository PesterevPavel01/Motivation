using Microsoft.Extensions.DependencyInjection;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Interfaces;
using Motivation.KpiCalculator.Core.Filters;
using Motivation.KpiCalculator.Core.Interfaces;
using Motivation.ReceivablesKpiCalculator.Filters;
using Motivation.ReceivablesKpiCalculator.Providers;
using Motivation.ReceivablesKpiCalculator.Strategies;

namespace Motivation.ReceivablesKpiCalculator.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddReceivablesKpiCalculator(this IServiceCollection source)
    {
        source.AddTransient<ICalculator, ReceivablesKpiCalculator>();
        source.AddScoped<IKpiQueryBuilder<ReceivableDto>, ReceivableKpiQueryBuilder>();
        source.AddScoped<ITransactionDataProvider<ReceivableDto>, ReceivablesDataProvider>();
        source.AddScoped<IFilter<ReceivableDto>, ReceivablesCustomerFilter>();
        source.AddScoped<IFilter<ReceivableDto>, ReceivablesCompanyFilter>();
        source.AddScoped<IFilter<ReceivableDto>, ReceivablesCustomerManagerFilter>();
        source.AddScoped<ICalculationStrategy<ReceivableDto>, AmountReceivableCalculationStrategy>();
    }
}
