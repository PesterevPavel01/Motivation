using Calabonga.AspNetCore.AppDefinitions;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Motivation.Contracts.Dto;
using Motivation.HRService.Application.Messaging.HoursMessages;
using Motivation.IdentityServer.Domain.Base;

namespace Motivation.HRService.Endpoints;

public sealed class HoursEndpointsDefinition : AppDefinition
{
    public override void ConfigureApplication(WebApplication app)
        => app.MapHoursEndpoints();
}

internal static class HoursEndpointsDefinitionExtensions
{
    public static void MapHoursEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/hours").WithTags("Hours");

        group.MapPost("standard/create", async ([FromServices] IMediator mediator, [FromBody] StandardHoursCreateDto hours, HttpContext context)
                =>
        {
            var result = await mediator.Send(new PostHours.Request(hours), context.RequestAborted);
            if (!result.Ok)
                return Results.BadRequest(result.Error);
            return Results.Ok(result.Result);
        })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

        group.MapGet("standard/all", async ([FromServices] IMediator mediator, HttpContext context)
            =>
        {
            var result = await mediator.Send(new GetHours.Request(), context.RequestAborted);
            if (!result.Ok)
                return Results.BadRequest(result.Error);
            return Results.Ok(result.Result);
        })
        .RequireAuthorization(AppData.SystemAdministratorRoleName)
        .Produces(200)
        .ProducesProblem(401)
        .WithOpenApi();

        group.MapPut("standard/{id:guid}", async (Guid id, [FromServices] IMediator mediator, StandardHoursUpdateDto model, HttpContext context)
            => 
        { 
            var result = await mediator.Send(new PutHours.Request(id, model), context.RequestAborted);
            if (!result.Ok)
                return Results.BadRequest(result.Error);
            return Results.Ok(result.Result);
        })
        .RequireAuthorization(AppData.SystemAdministratorRoleName)
        .Produces(200)
        .ProducesProblem(401)
        .WithOpenApi();

        group.MapDelete("standard/{id:guid}", async (Guid id, [FromServices] IMediator mediator, HttpContext context)
            =>
        {
            var result = await mediator.Send(new DeleteHours.Request(id), context.RequestAborted);
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
