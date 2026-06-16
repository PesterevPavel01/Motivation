using Calabonga.AspNetCore.AppDefinitions;
using Motivation.Contracts.Interfaces;
using Motivation.Domain.DomainEvents.EmployeeDomainEvents;
using Motivation.Domain.DomainEvents.PositionDomainEvents;
using Motivation.Domain.Events.EmployeeDomainEvents;
using Motivation.Domain.Events.PositionDomainEvents;
using Motivation.Infrastructure.Options;
using Motivation.Infrastructure.Processors;

namespace Motivation.HRService.Definitions.Common
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
                    typeof(PositionCreatedDomainEvent).Name,
                    typeof(PositionUpdatedDomainEvent).Name,
                    typeof(EmployeeCreatedDomainEvent).Name,
                    typeof(EmployeeUpdatedDomainEvent).Name,
                    typeof(AssignEmployeeToPositionDomainEvent).Name,
                    typeof(SetActualHoursDomainEvent).Name,
                    typeof(RemoveActualHoursDomainEvent).Name,
                    typeof(SetDeductionDomainEvent).Name,
                    typeof(RemoveDeductionDomainEvent).Name,
                    typeof(SetExtraPartDomainEvent).Name,
                    typeof(RemoveExtraPartDomainEvent).Name,
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