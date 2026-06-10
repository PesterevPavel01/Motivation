using Calabonga.OperationResults;
using Mediator;
using Microsoft.AspNetCore;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using System.Text.Json;

namespace Motivation.IdentityServer.Web.Application.Messaging.EventItemMessages.Queries;

public class GetClientCredentialsToken
{
    public record Request(HttpContext HttpContext) : IRequest<Operation<ClaimsPrincipal, string>>;

    public class Handler(IOpenIddictApplicationManager applicationManager)
        : IRequestHandler<Request, Operation<ClaimsPrincipal, string>>
    {
        public async ValueTask<Operation<ClaimsPrincipal, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var oidcRequest = request.HttpContext.GetOpenIddictServerRequest();
            if (oidcRequest is null || !oidcRequest.IsClientCredentialsGrantType())
            {
                return Operation.Error("The OpenID Connect request cannot be retrieved.");
            }

            var clientId = oidcRequest.ClientId;
            if (string.IsNullOrWhiteSpace(clientId))
            {
                return Operation.Error("Client ID is required");
            }

            var client = await applicationManager.FindByClientIdAsync(clientId, cancellationToken);
            if (client is null)
            {
                return Operation.Error("Client not found");
            }

            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(OpenIddictConstants.Claims.Subject, clientId)
                .SetDestinations(OpenIddictConstants.Destinations.AccessToken));
            identity.AddClaim(new Claim(OpenIddictConstants.Claims.ClientId, clientId)
                .SetDestinations(OpenIddictConstants.Destinations.AccessToken));

            var propertiesProp = client.GetType().GetProperty("Properties");
            if (propertiesProp is not null)
            {
                var propertiesJson = propertiesProp.GetValue(client) as string;
                if (!string.IsNullOrEmpty(propertiesJson))
                {
                    var properties = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(propertiesJson);
                    if (properties is not null && properties.TryGetValue("roles", out var rolesElement))
                    {
                        var roles = JsonSerializer.Deserialize<string[]>(rolesElement.GetRawText()) ?? Array.Empty<string>();
                        foreach (var role in roles.Where(r => !string.IsNullOrEmpty(r)))
                        {
                            identity.AddClaim(new Claim(OpenIddictConstants.Claims.Role, role)
                                .SetDestinations(OpenIddictConstants.Destinations.AccessToken));
                        }
                    }
                }
            }

            // Add scopes
            foreach (var scope in oidcRequest.GetScopes())
            {
                identity.AddClaim(new Claim(OpenIddictConstants.Claims.Scope, scope)
                    .SetDestinations(OpenIddictConstants.Destinations.AccessToken));
            }

            var claimsPrincipal = new ClaimsPrincipal(identity);
            claimsPrincipal.SetScopes(oidcRequest.GetScopes());

            return claimsPrincipal;
        }
    }
}
