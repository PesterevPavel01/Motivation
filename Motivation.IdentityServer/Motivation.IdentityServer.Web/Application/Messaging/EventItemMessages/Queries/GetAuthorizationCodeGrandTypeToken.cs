using Calabonga.OperationResults;
using Mediator;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;

namespace Motivation.IdentityServer.Web.Application.Messaging.EventItemMessages.Queries;

public class GetAuthorizationCodeGrandTypeToken { 
    public record Request(HttpContext HttpContext) : IRequest<Operation<ClaimsPrincipal, string>>;

    public class Handler(IOpenIddictApplicationManager applicationManager)
        : IRequestHandler<Request, Operation<ClaimsPrincipal, string>>
    {
        public async ValueTask<Operation<ClaimsPrincipal, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var httpContext = request.HttpContext;

            var oidcRequest = httpContext.GetOpenIddictServerRequest();
            if (oidcRequest == null || !oidcRequest.IsAuthorizationCodeGrantType())
            {
                return Operation.Error("Invalid grant type");
            }

            var authenticateResult = await httpContext.AuthenticateAsync(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded || authenticateResult.Principal is null)
            {
                return Operation.Error("Authentication failed");
            }

            return authenticateResult.Principal;
        }
    }
}
