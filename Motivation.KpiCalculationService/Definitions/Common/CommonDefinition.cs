using Calabonga.AspNetCore.AppDefinitions;
using Motivation.AnalyticsCore.Application.Services;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Infrastructure.Processors;
using Motivation.KpiCalculationService.Application.Executor;
using Motivation.KpiCalculationService.Application.Executor.Steps;
using Motivation.KpiCalculationService.Application.Interfaces;

namespace Motivation.KpiCalculationService.Definitions.Common
{
    /// <summary>
    /// AspNetCore common configuration
    /// </summary>
    public class CommonDefinition : AppDefinition
    {
        /// <summary>
        /// Configure application for current microservice
        /// </summary>
        /// <param name="app"></param>
        public override void ConfigureApplication(WebApplication app)
            => app.UseHttpsRedirection();

        /// <summary>
        /// Configure services for current microservice
        /// </summary>
        /// <param name="builder"></param>
        public override void ConfigureServices(WebApplicationBuilder builder)
        {
            /*builder.Services.Configure<OutboxProcessorOptions>(options =>
            {
                options.SupportedEventTypes = [
                    typeof(AssignKpiToPositionDomainEvent).Name,
                    typeof(KpiCreatedDomainEvent).Name,
                    typeof(KpiDeletedDomainEvent).Name,
                    typeof(KpiRemovedFromPositionDomainEvent).Name,
                    typeof(KpiUpdatedDomainEvent).Name,
                ];
            });*/

            //builder.Services.AddSalesKpiCalculator();
            //builder.Services.AddComplexityKpiCalculator();
            builder.Services.AddScoped<IEntityServices<CompanyDto>, CompanyService>();
            builder.Services.AddScoped<IEntityServices<WarehouseDto>, WarehouseService>();
            builder.Services.AddScoped<IEntityServices<ProductDto>, ProductService>();
            builder.Services.AddScoped<IEntityServices<CustomerDto>, CustomerService>();
            builder.Services.AddScoped<IEntityServices<ReceivableDto>, ReceivableService>();
            builder.Services.AddScoped<IEntityServices<CustomerTransactionDto>, CustomerTransactionService>();
            builder.Services.AddScoped<IEntityServices<InventoryBalanceDto>, InventoryBalanceService>();
            builder.Services.AddScoped<IEntityServices<WarehouseTransactionDto>, WarehouseTransactionService>();

            builder.Services.AddScoped<IExecutor<SyncDto>, SyncExecutor>();
            builder.Services.AddScoped<ISyncStep, SyncCompaniesStep>();
            builder.Services.AddScoped<ISyncStep, SyncCustomersStep>();
            builder.Services.AddScoped<ISyncStep, SyncProductsStep>();
            builder.Services.AddScoped<ISyncStep, SyncWarehouseStep>();
            builder.Services.AddScoped<ISyncStep, SyncReceivablesStep>();
            builder.Services.AddScoped<ISyncStep, SyncCustomerTransactionStep>();
            builder.Services.AddScoped<ISyncStep, SyncInventoryBalancesStep>();
            builder.Services.AddScoped<ISyncStep, SyncWarehousesTransactions>();

            builder.Services.AddScoped<IOutboxProcessor, OutboxProcessor>();
            builder.Services.AddScoped<IOutboxCleanerProcessor, OutboxCleanerProcessor>();

            builder.Services.AddLocalization();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddResponseCaching();
            builder.Services.AddMemoryCache();
        }
    }
}