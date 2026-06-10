using Calabonga.AspNetCore.AppDefinitions;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Motivation.IdentityServer.Domain.Base;
using Motivation.SynchronisationService.Web.Application.Messaging.ProfileMessages.Queries;

namespace Motivation.SynchronisationService.Web.Endpoints;

public sealed class ProfilesEndpointDefinition : AppDefinition
{
    public override void ConfigureApplication(WebApplication app)
        => app.MapProfilesEndpoints();
}

internal static class ProfilesEndpointDefinitionExtensions
{
    public static void MapProfilesEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/profiles").WithTags("Profiles");

        group.MapGet("roles", async ([FromServices] IMediator mediator, HttpContext context)
                => await mediator.Send(new GetProfile.Request(), context.RequestAborted))
            .RequireAuthorization(AppData.SystemAdministratorRoleName)
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();
    }
}
