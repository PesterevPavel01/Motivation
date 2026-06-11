using Calabonga.AspNetCore.AppDefinitions;
using Motivation.Application.HostedService;

namespace Motivation.KpiService.Definitions.HostedService
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