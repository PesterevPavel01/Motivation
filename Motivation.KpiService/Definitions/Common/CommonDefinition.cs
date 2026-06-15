using Calabonga.AspNetCore.AppDefinitions;
using Motivation.Contracts.Interfaces;
using Motivation.Domain.DomainEvents.KpiDomainEvents;
using Motivation.Domain.Events.KpiDomainEvents;
using Motivation.Infrastructure.Options;
using Motivation.Infrastructure.Processors;

namespace Motivation.KpiService.Definitions.Common
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
            builder.Services.Configure<OutboxProcessorOptions>(options =>
            {
                options.SupportedEventTypes = [
                    typeof(KpiCreatedDomainEvent).Name,
                    typeof(KpiUpdatedDomainEvent).Name,
                    typeof(AssignKpiToPositionDomainEvent).Name,
                ];
            });
            builder.Services.AddScoped<IOutboxProcessor, OutboxProcessor>();
            builder.Services.AddScoped<IOutboxCleanerProcessor, OutboxCleanerProcessor>();

            builder.Services.AddLocalization();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddResponseCaching();
            builder.Services.AddMemoryCache();
        }
    }
}