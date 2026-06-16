using Calabonga.AspNetCore.AppDefinitions;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Motivation.Contracts.Dto;
using Motivation.HRService.Application.Messaging.PositionMessages.Queries;
using Motivation.IdentityServer.Domain.Base;

namespace Motivation.HRService.Endpoints;

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

        group.MapPost("create", async ([FromServices] IMediator mediator, [FromBody] PositionCreateDto position, HttpContext context)
            =>
        {
            var result = await mediator.Send(new PostPosition.Request(position), context.RequestAborted);
            if (!result.Ok)
                return Results.BadRequest(result.Error);
            return Results.Ok(result.Result);
        })
        .RequireAuthorization(AppData.SystemAdministratorRoleName)
        .Produces(200)
        .ProducesProblem(401)
        .WithOpenApi();

        group.MapPut("{code}/update", async ([FromServices] IMediator mediator, string code,  [FromBody] PositionCreateDto position, HttpContext context)
            =>
        {
            var result = await mediator.Send(new PutPosition.Request(code, position), context.RequestAborted);
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
            var result = await mediator.Send(new GetPositions.Request(), context.RequestAborted);
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
