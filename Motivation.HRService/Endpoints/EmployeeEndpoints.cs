using Calabonga.AspNetCore.AppDefinitions;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Motivation.Contracts.Dto;
using Motivation.HRService.Application.Messaging.EmployeeMessages.Queries;
using Motivation.HRService.Application.Messaging.PositionAssigmentMessages;
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
                => await mediator.Send(new PostEmployee.Request(employee), context.RequestAborted))
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapPost("set-extra-part", async ([FromServices] IMediator mediator, [FromBody] ExtraPartDto extraPart, HttpContext context)
                => await mediator.Send(new SetExtraPart.Request(extraPart), context.RequestAborted))
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapPost("set-deduction", async ([FromServices] IMediator mediator, [FromBody] DeductionDto deduction, HttpContext context)
                => await mediator.Send(new SetDeductionPart.Request(deduction), context.RequestAborted))
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapGet("all", async ([FromServices] IMediator mediator, HttpContext context)
                => await mediator.Send(new GetEmployees.Request(), context.RequestAborted))
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();

            group.MapGet("by-first-name/{firstName}", async ([FromServices] IMediator mediator, [FromRoute] string firstName, HttpContext context)
                => await mediator.Send(new GetEmployeeByFirstName.Request(firstName), context.RequestAborted))
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();
        }
    }
}
