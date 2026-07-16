using Calabonga.AspNetCore.AppDefinitions;
using Motivation.Application.Settings;
using Motivation.Contracts.QueueEvents;
using Rebus.Config;
using Rebus.Serialization.Json;
using Serilog;

namespace Motivation.AnaliticsCore.SyncService.Definitions.Rebus
{
    public class RebusDefinition : AppDefinition
    {
        public override void ConfigureServices(WebApplicationBuilder builder)
        {
            var rabbitSettings = builder.Configuration.GetSection("RabbitMq").Get<RabbitSettings>();

            builder.Services.AddRebus(configure: config =>
            {
                config
                .Logging(x => x.Serilog(Log.Logger))
                .Serialization(x => x.UseSystemTextJson())
                .Transport(x => x.UseRabbitMq(rabbitSettings.RabbitUrl, "ISyncServiceQueueEvent"))
                .Options(x =>
                {
                    x.SetNumberOfWorkers(5);
                    x.SetBusName("KpiSyncService");
                });

                return config;
            }
            , onCreated: async bus =>
            {
                await bus.Subscribe<SourceDataSyncedQueueEvent>();
            }
            );

            builder.Services.AutoRegisterHandlersFromAssemblyOf<SyncServiceAssemblyReference>();
        }
    }
}