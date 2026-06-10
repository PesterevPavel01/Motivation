using Calabonga.AspNetCore.AppDefinitions;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Motivation.Contracts.Dto;
using Motivation.IdentityServer.Domain.Base;
using Motivation.SynchronisationService.Web.Application.Messaging.Kpis.Queries;
using Motivation.SynchronisationService.Web.Application.Messaging.PositionMessages.Queries;
using System.Reflection.Emit;

namespace Motivation.SynchronisationService.Web.Endpoints;

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

        group.MapPost("create", async ([FromServices] IMediator mediator,[FromBody] PositionCreateDto position, HttpContext context)
                => await mediator.Send(new PostPosition.Request(position), context.RequestAborted))
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

        group.MapPost("/add-kpi", async ([FromBody] AssignKpiToPositionDto assignModel, [FromServices] IMediator mediator, HttpContext context)
            => await mediator.Send(new AssignKpi.Request(assignModel), context.RequestAborted))
        .RequireAuthorization(AppData.SystemAdministratorRoleName)
        .Produces(200)
        .ProducesProblem(401)
        .WithOpenApi();

        group.MapGet("all", async ([FromServices] IMediator mediator, HttpContext context)
            => await mediator.Send(new GetPositions.Request(), context.RequestAborted))
        .RequireAuthorization(AppData.SystemAdministratorRoleName)
        .Produces(200)
        .ProducesProblem(401)
        .WithOpenApi();
    }
}
