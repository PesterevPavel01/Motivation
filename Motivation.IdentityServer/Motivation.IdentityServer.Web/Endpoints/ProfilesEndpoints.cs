using Calabonga.AspNetCore.AppDefinitions;
using Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Motivation.IdentityServer.Web.Application.Messaging.ProfileMessages.Queries;
using Motivation.IdentityServer.Web.Application.Messaging.ProfileMessages.ViewModels;

namespace Motivation.IdentityServer.Web.Endpoints
{
    public sealed class ProfilesEndpointDefinition : AppDefinition
    {
        public override void ConfigureApplication(WebApplication app)
        {
            var group = app.MapGroup("/api/profiles").WithTags("Profiles");

            group.MapGet("roles", async ([FromServices] IMediator mediator, HttpContext context)
                    => await mediator.Send(new GetProfile.Request(), context.RequestAborted))
                .RequireAuthorization(new AuthorizeAttribute
                {
                    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                })
                .Produces(200)
                .ProducesProblem(401)
                .WithOpenApi();

            group.MapPost("register", async ([FromServices] IMediator mediator, [FromBody] RegisterViewModel model, HttpContext context)
                    => await mediator.Send(new RegisterAccount.Request(model), context.RequestAborted))
                .Produces(200)
                .ProducesProblem(401)
                .Produces<RegisterViewModel>()
                .WithOpenApi()
                .AllowAnonymous();
        }
    }
}
