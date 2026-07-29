using Microsoft.Extensions.DependencyInjection;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.KpiCalculator.Core.Filters;
using Motivation.KpiCalculator.Core.Interfaces;
using Motivation.KpiCalculator.Core.Providers;
using Motivation.KpiCalculator.Core.Services;
using Motivation.KpiCalculator.Core.Strategies;

namespace Motivation.KpiCalculator.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddKpiCalculator(this IServiceCollection source)
    {
        source.AddScoped<IHierarchyService, HierarchyService>();
        //source.AddScoped<ICalculator, BaseKpiCalculator<CustomerTransactionDto>>();
        //source.AddScoped<IKpiQueryBuilder<CustomerTransactionDto>, BaseKpiQueryBuilder<CustomerTransactionDto>>();
        source.AddScoped<ICalculationStrategy<CustomerTransactionDto>, AmountCalculationStrategy>();
        source.AddScoped<ICalculationStrategy<CustomerTransactionDto>, QuantityCalculationStrategy>();
        source.AddScoped<ITransactionDataProvider<CustomerTransactionDto>, TransactionDataProvider>();
        source.AddScoped<IFilter<CustomerTransactionDto>, CustomerTransactionResponsibleFilter>();
        source.AddScoped<IFilter<CustomerTransactionDto>, CustomerTransactionProductFilter>();
        source.AddScoped<IFilter<CustomerTransactionDto>, CustomerTransactionProductResponsibleFilter>();
        source.AddScoped<IFilter<CustomerTransactionDto>, CustomerTransactionProductStatusFilter>();
        source.AddScoped<IFilter<CustomerTransactionDto>, CustomerTransactionProductSupplierFilter>();
        source.AddScoped<IFilter<CustomerTransactionDto>, CustomerTransactionsCustomerFilter>();
        source.AddScoped< IFilter<CustomerTransactionDto>, CustomerTransactionCustomerManagerFilter>();
        source.AddScoped<IFilter<CustomerTransactionDto>, CustomerTransactionsCompanyFilter>();
    }
}
