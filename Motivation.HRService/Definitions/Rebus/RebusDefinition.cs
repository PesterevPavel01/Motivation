using Calabonga.AspNetCore.AppDefinitions;
using Motivation.Application.Settings;
using Motivation.Contracts.Events;
using Motivation.Domain.DomainEvents.PositionDomainEvents;
using Rebus.Config;
using Rebus.Serialization.Json;
using Serilog;

namespace Motivation.HRService.Definitions.Rebus;

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
            .Transport(x => x.UseRabbitMq(rabbitSettings.RabbitUrl, "IHRServiceQueueEvent"))
            .Options(x =>
            {
                x.SetNumberOfWorkers(5);
                x.SetBusName("HRService");
            });

            return config;
        }
        , onCreated: async bus =>
            {
                await bus.Subscribe<PositionUpdatedQueueEvent>();
            }
        );

        builder.Services.AutoRegisterHandlersFromAssemblyOf<HRServiceAssemblyReference>();
    }
}

