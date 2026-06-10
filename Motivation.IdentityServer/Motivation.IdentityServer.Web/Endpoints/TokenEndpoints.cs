
using Calabonga.AspNetCore.AppDefinitions;
using Mediator;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Motivation.IdentityServer.Infrastructure;
using Motivation.IdentityServer.Web.Application.Messaging.EventItemMessages.Queries;
using Motivation.IdentityServer.Web.Application.Services;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Motivation.IdentityServer.Web.Endpoints
{
    /// <summary>
    /// Token Endpoint for OpenIddict
    /// </summary>
    public sealed class TokenEndpoints : AppDefinition
    {
        public override void ConfigureApplication(WebApplication app) =>
            app.MapPost("~/connect/token", async (
                        [FromServices] IMediator mediator,
                        HttpContext httpContext,
                        UserManager<ApplicationUser> userManager,
                        SignInManager<ApplicationUser> signInManager,
                        IOpenIddictApplicationManager applicationManager,
                        IAccountService accountService) =>
            {
                var request = httpContext.GetOpenIddictServerRequest() 
                    ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

                if (request.IsClientCredentialsGrantType())
                {
                    var result = await mediator.Send(new GetClientCredentialsToken.Request(httpContext));
                    if (!result.Ok)
                    {
                        return Results.BadRequest(new { error = result.Error });
                    }
                    return Results.SignIn(result.Result!, new AuthenticationProperties(),
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }

                if (request.IsPasswordGrantType())
                {
                    var result = await mediator.Send(new GetPasswordGrandTypeToken.Request(httpContext));

                    if (!result.Ok)
                    {
                        return Results.BadRequest(new { error = result.Error });
                    }

                    return Results.SignIn(result.Result!, null,
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }

                if (request.IsAuthorizationCodeGrantType())
                {
                    var result = await mediator.Send(new GetAuthorizationCodeGrandTypeToken.Request(httpContext));

                    if (!result.Ok)
                    {
                        return Results.BadRequest(new { error = result.Error });
                    }

                    return Results.SignIn(result.Result!, null,
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }

                return Results.Problem("The specified grant type is not supported.");
            })
                //.ExcludeFromDescription()
                .AllowAnonymous();
    }
}
