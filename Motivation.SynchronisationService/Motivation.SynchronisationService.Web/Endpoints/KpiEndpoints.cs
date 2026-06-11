using Calabonga.AspNetCore.AppDefinitions;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Motivation.Contracts.Dto;
using Motivation.IdentityServer.Domain.Base;
using Motivation.SynchronisationService.Web.Application.Messaging.KpiMessages.Queries;
using Motivation.SynchronisationService.Web.Application.Messaging.Kpis.Queries;

namespace Motivation.SynchronisationService.Web.Endpoints;

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

        group.MapPost("sales/create", async ([FromServices] IMediator mediator,[FromBody] KpiCreateDto kpi, HttpContext context)
                => await mediator.Send(new PostSalesKpi.Request(kpi), context.RequestAborted))
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

        group.MapPost("unit/create", async ([FromServices] IMediator mediator, [FromBody] UnitCreateDto kpi, HttpContext context)
            => await mediator.Send(new PostUnit.Request(kpi), context.RequestAborted))
        .RequireAuthorization(AppData.SystemAdministratorRoleName)
        .Produces(200)
        .ProducesProblem(401)
        .WithOpenApi();

        group.MapGet("all", async ([FromServices] IMediator mediator, HttpContext context)
            => await mediator.Send(new GetKpis.Request(), context.RequestAborted))
        .RequireAuthorization(AppData.SystemAdministratorRoleName)
        .Produces(200)
        .ProducesProblem(401)
        .WithOpenApi();
    }
}
