using Calabonga.AspNetCore.AppDefinitions;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Motivation.Contracts.Dto;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.IdentityServer.Domain.Base;
using Motivation.KpiService.Application.Messaging.Executors;

namespace Motivation.KpiService.Endpoints;

public class KpiExecutorEndpointDefinition : AppDefinition
{
    public override void ConfigureApplication(WebApplication app)
        => app.KpiExecutorEndpoints();
}

internal static class KpiExecutorEndpointDefinitionExtensions
{
    public static void KpiExecutorEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/calculate").WithTags("Calculate");

        group.MapPost("execute", async ([FromServices] IMediator mediator, [FromBody] CalculateKpiDto model, HttpContext context)
            =>
        {
            var result = await mediator.Send(new CalculateEmployeeKpiCommand.Request(model), context.RequestAborted);
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
