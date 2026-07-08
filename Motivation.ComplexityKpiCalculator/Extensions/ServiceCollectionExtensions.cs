using Microsoft.Extensions.DependencyInjection;
using Motivation.Contracts.Interfaces;

namespace Motivation.KpiComplexityCalculator.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddComplexityKpiCalculator(this IServiceCollection source)
    {
        source.AddScoped<ICalculator, BasketComplexityKpiCalculator>();
    }
}
