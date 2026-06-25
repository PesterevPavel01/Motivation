using Calabonga.AspNetCore.AppDefinitions;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Motivation.Contracts.Dto;
using Motivation.HRService.Application.Messaging.EmployeeMessages.Queries;
using Motivation.IdentityServer.Domain.Base;

namespace Motivation.HRService.Endpoints
{
    public sealed class EmployeeEndpointsDefinition : AppDefinition
    {
        public override void ConfigureApplication(WebApplication app)
            => app.MapEmployeeEndpoints();
    }

    internal static class EmployeeEndpointsDefinitionExtensions
    {
        public static void MapEmployeeEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/employees").WithTags("Employees");

            group.MapPost("create", async ([FromServices] IMediator mediator, [FromBody] EmployeeCreateDto employee, HttpContext context)
                =>
            {
                var result = await mediator.Send(new PostEmployee.Request(employee), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapPut("{employeeCode}/update", async ([FromServices] IMediator mediator, string employeeCode, [FromBody] EmployeeUpdateDto employee, HttpContext context)
                =>
            {
                var result = await mediator.Send(new PutEmployee.Request(employeeCode, employee), context.RequestAborted);
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
                var result = await mediator.Send(new DeleteEmployee.Request(code), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapPut("{employeeCode}/set-extra-part", async ([FromServices] IMediator mediator, string employeeCode, [FromBody] CreateExtraPartDto extraPart, HttpContext context)
                =>
            {
                var result = await mediator.Send(new SetExtraPart.Request(employeeCode, extraPart), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapDelete("{employeeCode}/remove-extra-part", async ([FromServices] IMediator mediator, string employeeCode, [FromBody] RemoveExtraPartDto model, HttpContext context)
                =>
            {
                var result = await mediator.Send(new RemoveExtraPart.Request(employeeCode, model), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapPut("{employeeCode}/set-deduction", async ([FromServices] IMediator mediator, string employeeCode, [FromBody] SetDeductionDto deduction, HttpContext context)
                =>
            {
                var result = await mediator.Send(new SetDeduction.Request(employeeCode, deduction), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapDelete("{employeeCode}/remove-deduction", async ([FromServices] IMediator mediator, string employeeCode, [FromBody] RemoveDeductionDto model, HttpContext context)
                =>
            {
                var result = await mediator.Send(new RemoveDeduction.Request(employeeCode, model), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapPut("{employeeCode}/set-actual-hours", async ([FromServices] IMediator mediator, string employeeCode, [FromBody] SetActualHoursDto actualHours, HttpContext context)
                =>
            {
                var result = await mediator.Send(new SetActualHours.Request(employeeCode, actualHours), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapDelete("{employeeCode}/remove-actual-hours", async ([FromServices] IMediator mediator, string employeeCode, [FromBody] RemoveActualHoursDto actualHours, HttpContext context)
                =>
            {
                var result = await mediator.Send(new RemoveActualHours.Request(employeeCode, actualHours), context.RequestAborted);
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
                var result = await mediator.Send(new GetEmployees.Request(), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapGet("by-first-name/{firstName}", async ([FromServices] IMediator mediator, [FromRoute] string firstName, HttpContext context)
                =>
            {
                var result = await mediator.Send(new GetEmployeeByFirstName.Request(firstName), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapPut("{employeeCode}/assign", async ([FromServices] IMediator mediator,
                [FromBody] AssignEmployeeToPositionDto model,
                string employeeCode,
                HttpContext context)
                =>
            {
                var result = await mediator.Send(new AssignEmployeeToPosition.Request(employeeCode, model), context.RequestAborted);
                if (!result.Ok)
                    return Results.BadRequest(result.Error);
                return Results.Ok(result.Result);
            })
             .RequireAuthorization(AppData.SystemAdministratorRoleName)
             .Produces(200)
             .ProducesProblem(401)
             .WithOpenApi();

            group.MapDelete("{employeeCode}/dismiss/{positionCode}", async (
                [FromServices] IMediator mediator,
                string employeeCode,
                string positionCode,
                HttpContext context)
                =>
            {
                var result = await mediator.Send(new DismissEmployeeFromPosition.Request(employeeCode, positionCode), context.RequestAborted);
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
