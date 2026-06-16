using Calabonga.AspNetCore.AppDefinitions;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Motivation.Contracts.Dto;
using Motivation.IdentityServer.Domain.Base;
using Motivation.KpiService.Application.Messaging.KpiMessages.Queries;
using Motivation.KpiService.Application.Messaging.Kpis.Queries;

namespace Motivation.KpiService.Endpoints
{
    public sealed class KpiEndpointsDefinition : AppDefinition
    {
        public override void ConfigureApplication(WebApplication app)
            => app.MapKpiEndpoints();
    }

    internal static class KpiEndpointsDefinitionExtensions
    {
        public static void MapKpiEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/kpi").WithTags("Kpis");

            group.MapPost("sales/create", async ([FromServices] IMediator mediator, [FromBody] KpiCreateDto kpi, HttpContext context)
                =>
            {
                var result = await mediator.Send(new PostSalesKpi.Request(kpi), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapPut("sales/update/{code}", async ([FromServices] IMediator mediator, string code, [FromBody] KpiCreateDto model, HttpContext context)
                =>
            {
                var result = await mediator.Send(new PutSalesKpi.Request(code, model), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapPost("unit/create", async ([FromServices] IMediator mediator, [FromBody] UnitCreateDto kpi, HttpContext context)
                =>
            {
                var result = await mediator.Send(new PostUnit.Request(kpi), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapGet("all", async ([FromServices] IMediator mediator, HttpContext context)
                =>
            {
                var result = await mediator.Send(new GetKpis.Request(), context.RequestAborted);
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
