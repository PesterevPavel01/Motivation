using Microsoft.Extensions.DependencyInjection;
using Motivation.Contracts.Interfaces;
using Motivation.Contracts.Models;
using Motivation.KpiCalculator.Core.Interfaces;
using Motivation.ManualKpiCalculator.Providers;
using Motivation.ManualKpiCalculator.Strategies;

namespace Motivation.ManualKpiCalculator.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddManualKpiCalculator(this IServiceCollection source)
    {
        source.AddTransient<ICalculator, ManualKpiCalculator>();
        source.AddScoped<IKpiQueryBuilder<ManualResultDto>, ManualKpiQueryBuilder>();
        source.AddScoped<ICalculationStrategy<ManualResultDto>, ManualCalculationStrategy>();
        source.AddScoped<ITransactionDataProvider<ManualResultDto>, ManualTransactionDataProvider>();
    }
}
