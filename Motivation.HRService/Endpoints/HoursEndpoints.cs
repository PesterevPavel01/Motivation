using Calabonga.AspNetCore.AppDefinitions;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Motivation.Application.Mappings;
using Motivation.Contracts;
using Motivation.Contracts.Dto;
using Motivation.Domain.Enum;
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
        var group = routes.MapGroup("/api/standard-hours").WithTags("StandardHours");

        group.MapPost("create", async ([FromServices] IMediator mediator, [FromBody] StandardHoursCreateDto hours, HttpContext context)
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

        group.MapGet("all", async ([FromServices] IMediator mediator, HttpContext context)
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

        group.MapGet("by-month/{month:int}/{year:int}/{workWeekType}", async ([FromServices] IMediator mediator, int month, int year, string workWeekType, HttpContext context)
            =>
        {
            var result = await mediator.Send(new GetHours.Request(workWeekType, month, year), context.RequestAborted);
            if (!result.Ok)
                return Results.BadRequest(result.Error);

            var currentModel = result.Result.FirstOrDefault();
            if(currentModel is null)
                return Results.BadRequest(ErrorMessages.NotFound);

            return Results.Ok(currentModel);
        })
        .RequireAuthorization(AppData.SystemAdministratorRoleName)
        .Produces(200)
        .ProducesProblem(401)
        .WithOpenApi();

        group.MapPut("{id:guid}/update", async (Guid id, [FromServices] IMediator mediator, StandardHoursUpdateDto model, HttpContext context)
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

        group.MapDelete("{id:guid}", async (Guid id, [FromServices] IMediator mediator, HttpContext context)
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
