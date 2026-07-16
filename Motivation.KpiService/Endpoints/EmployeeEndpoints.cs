using Calabonga.AspNetCore.AppDefinitions;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Motivation.Contracts.Dto;
using Motivation.IdentityServer.Domain.Base;
using Motivation.KpiService.Application.Messaging.Employees;
using Motivation.KpiService.Application.Messaging.Kpis;
using Motivation.KpiService.Application.Messaging.Targets;

namespace Motivation.KpiService.Endpoints;

public sealed class EmployeeEndpointsDefinition : AppDefinition
{
    public override void ConfigureApplication(WebApplication app)
        => app.MapEmployeeEndpoints();
}

internal static class EmployeeEndpointsDefinitionExtensions
{
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/employee").WithTags("Employees");

        group.MapGet("{employeeCode}/kpis", async ([FromServices] IMediator mediator, string employeeCode, HttpContext context)
            =>
        {
            var result = await mediator.Send(new GetEmployeeKpis.Request(employeeCode), context.RequestAborted);
            if (!result.Ok)
                return Results.BadRequest(result.Error);
            return Results.Ok(result.Result);
        })
        .RequireAuthorization(AppData.SystemAdministratorRoleName)
        .Produces(200)
        .ProducesProblem(401)
        .WithOpenApi();

        group.MapPut("kpi/set-target", async ([FromServices] IMediator mediator, [FromBody] SetEmployeeKpiTarget model, HttpContext context)
            =>
        {
            var result = await mediator.Send(new PutTarget.Request(model), context.RequestAborted);
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