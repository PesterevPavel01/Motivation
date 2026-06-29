using Calabonga.AspNetCore.AppDefinitions;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Motivation.Contracts.Dto;
using Motivation.IdentityServer.Domain.Base;
using Motivation.KpiService.Application.Messaging.Kpis;

namespace Motivation.KpiService.Endpoints
{
    public sealed class PositionEndpointsDefinition : AppDefinition
    {
        public override void ConfigureApplication(WebApplication app)
            => app.MapPositionEndpoints();
    }

    internal static class PositionEndpointsDefinitionExtensions
    {
        public static void MapPositionEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/positions").WithTags("Positions");

            group.MapPut("/{positionCode}/assign-kpis", async (string positionCode, [FromBody] IEnumerable<AssignKpiToPositionDto> assignModels, [FromServices] IMediator mediator, HttpContext context)
                =>
            {
                var result = await mediator.Send(new AssignKpi.Request(positionCode, assignModels), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapDelete("/{positionCode}/remove-kpi/{kpiCode}", async (string positionCode, string kpiCode, [FromServices] IMediator mediator, HttpContext context)
                =>
            {
                var result = await mediator.Send(new RemoveKpiFromPositionMessage.Request(positionCode, kpiCode), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();
        }

    }
}
