using Calabonga.AspNetCore.AppDefinitions;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Motivation.Contracts.Dto;
using Motivation.IdentityServer.Domain.Base;
using Motivation.KpiService.Application.Messaging.Kpis;

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

            group.MapPost("create", async ([FromServices] IMediator mediator, [FromBody] KpiCreateDto kpi, HttpContext context)
                =>
            {
                var result = await mediator.Send(new PostKpi.Request(kpi), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

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


            group.MapGet("unit/all", async ([FromServices] IMediator mediator, HttpContext context)
                =>
            {
                var result = await mediator.Send(new GetUnits.Request(), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapGet("calculationType/all", async ([FromServices] IMediator mediator, HttpContext context)
                =>
            {
                var result = await mediator.Send(new GetCalculationTypes.Request(), context.RequestAborted);
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

            group.MapDelete("{code}", async ([FromServices] IMediator mediator, string code, HttpContext context)
                =>
            {
                var result = await mediator.Send(new DeleteKpi.Request(code), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapPut("{code}/update", async ([FromServices] IMediator mediator, string code, [FromBody] KpiUpdateDto model, HttpContext context)
                =>
            {
                var result = await mediator.Send(new PutKpi.Request(model), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapGet("types", async ([FromServices] IMediator mediator, HttpContext context)
                =>
            {
                var result = await mediator.Send(new GetKpiTypes.Request(), context.RequestAborted);
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
