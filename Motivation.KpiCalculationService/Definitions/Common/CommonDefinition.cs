using Calabonga.AspNetCore.AppDefinitions;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Infrastructure.Processors;
using Motivation.KpiCalculator.Core.Extensions;
using Motivation.KpiComplexityCalculator.Extensions;
using Motivation.KpiComplexityCalculator.Models;
using Motivation.KpiSalesCalculator.Extensions;
using Motivation.ReceivablesKpiCalculator.Extensions;

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

            builder.Services.Configure<ComplexitySettings>(
            builder.Configuration.GetSection("ComplexitySettings"));
            builder.Services.AddKpiCalculator();
            builder.Services.AddSalesKpiCalculator();
            builder.Services.AddComplexityKpiCalculator();
            builder.Services.AddReceivablesKpiCalculator();

            builder.Services.AddScoped<IOutboxProcessor, OutboxProcessor>();
            builder.Services.AddScoped<IOutboxCleanerProcessor, OutboxCleanerProcessor>();

            builder.Services.AddLocalization();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddResponseCaching();
            builder.Services.AddMemoryCache();
        }
    }
}