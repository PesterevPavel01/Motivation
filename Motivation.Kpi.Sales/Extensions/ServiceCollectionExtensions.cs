using Microsoft.Extensions.DependencyInjection;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Interfaces;
using Motivation.KpiCalculator.Core;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiSalesCalculator.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddSalesKpiCalculator(this IServiceCollection source)
    {
        source.AddTransient<ICalculator, SalesKpiCalculator>();
        source.AddScoped<IKpiQueryBuilder<CustomerTransactionDto>, SalesKpiQueryBuilder>();
    }
}
