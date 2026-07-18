using Calabonga.AspNetCore.AppDefinitions;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Motivation.AnaliticsCore.SyncService.Application.Messaging;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.QueueEvents;

namespace Motivation.AnalyticsCore.KpiCalculationService.Endpoints
{
    public sealed class SyncEndpointsDefinition : AppDefinition
    {
        public override void ConfigureApplication(WebApplication app)
            => app.MapSyncEndpoints();
    }

    internal static class SyncEndpointsDefinitionExtensions
    {
        public static void MapSyncEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/sync").WithTags("Sync");
            group.MapPost("source", async ([FromServices] IMediator mediator, ILogger<SyncEndpointsDefinition> logger,  [FromBody] SyncDto syncModel, HttpContext context)
                =>
            {
                var result = await mediator.Send(new SyncSourceData.Request(syncModel), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            //.RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();
        }
    }
}
