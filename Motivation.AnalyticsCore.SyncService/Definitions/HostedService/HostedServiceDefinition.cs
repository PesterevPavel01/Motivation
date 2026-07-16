using Calabonga.AspNetCore.AppDefinitions;
using Motivation.AnalyticsCore.Application.HostedService;

namespace Motivation.AnaliticsCore.SyncService.Definitions.HostedService
{
    public class HostedServiceDefinition : AppDefinition
    {
        /// <summary>
        /// Configure services for current application
        /// </summary>
        /// <param name="builder"></param>
        public override void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddHostedService<OutboxProcessorExecutorHostedService>();
            builder.Services.AddHostedService<OutboxCleanerProcessorExecutorHostedService>();
        }
    }
}