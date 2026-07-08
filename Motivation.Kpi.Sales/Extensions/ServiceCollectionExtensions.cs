using Microsoft.Extensions.DependencyInjection;
using Motivation.Contracts.Interfaces;

namespace Motivation.KpiSalesCalculator.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddSalesKpiCalculator(this IServiceCollection source)
    {
        source.AddScoped<ICalculator, SalesKpiCalculator>();
    }
}
