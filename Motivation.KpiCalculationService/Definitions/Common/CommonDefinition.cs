using Calabonga.AspNetCore.AppDefinitions;
using Motivation.AnalyticsCore.Application.Services;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Infrastructure.Processors;

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
            builder.Services.AddScoped<IEntityServices<Company, CompanyDto>, CompanyService>();
            builder.Services.AddScoped<IEntityServices<Product, ProductDto>, ProductService>();
            builder.Services.AddScoped<IEntityServices<Customer, CustomerDto>, CustomerService>();
            builder.Services.AddScoped<IEntityServices<Receivable, ReceivableDto>, ReceivableService>();
            builder.Services.AddScoped<IEntityServices<CustomerTransaction, CustomerTransactionDto>, CustomerTransactionService>();
            builder.Services.AddScoped<IOutboxProcessor, OutboxProcessor>();
            builder.Services.AddScoped<IOutboxCleanerProcessor, OutboxCleanerProcessor>();

            builder.Services.AddLocalization();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddResponseCaching();
            builder.Services.AddMemoryCache();
        }
    }
}