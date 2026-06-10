using Microsoft.Extensions.Options;
using Motivation.IdentityServer.Domain.Base;
using Motivation.IdentityServer.Domain.Configuration;
using Motivation.IdentityServer.Domain.Interfaces;
using OpenIddict.Abstractions;

namespace Motivation.IdentityServer.Infrastructure.Configurations.IdentityServerClients;

public sealed class UserToServiceClientConfiguration : IIdentityClientConfiguration<OpenIddictApplicationDescriptor>
{
    private readonly AuthServerConfiguration _authConfig;

    public UserToServiceClientConfiguration(IOptions<AuthServerConfiguration> authConfig)
    {
        _authConfig = authConfig.Value;
    }

    public OpenIddictApplicationDescriptor GetDescriptor()
    {
        if (!_authConfig.Clients.TryGetValue("UserToService", out var clientConfig))
        {
            throw new InvalidOperationException("UserToService client is not configured");
        }

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = clientConfig.ClientId,
            ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
            ClientSecret = clientConfig.ClientSecret,
            DisplayName = "API testing clients with Authorization Code Flow demonstration",
            RedirectUris = 
            {
                new Uri("https://www.thunderclient.com/oauth/callback"),            // https://www.thunderclient.com/
                new Uri($"{_authConfig.Url}/swagger/oauth2-redirect.html"),         // https://swagger.io/ for IdentityModule as Example
                new Uri("https://localhost:20001/swagger/oauth2-redirect.html"),    // https://swagger.io/ for Module as Example
                new Uri("https://localhost:7207/signin-oidc"),                      // BlazorApp see folder ClientSamples in repository
                new Uri("https://localhost:30001/callback/login/local")             // RazorPagesUI
            },
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:7207/signout-callback-oidc"),             // Calabonga.BlazorApp see folder ClientSamples in repository
                new Uri("https://localhost:30001/callback/logout/local")             // Calabonga.Microservices.RazorPagesUI
            },
            Permissions =
            {
                // Endpoint permissions
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Introspection,
                OpenIddictConstants.Permissions.Endpoints.Token,

                // Grant type permissions
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                // Scope permissions
                OpenIddictConstants.Permissions.Prefixes.Scope + AppData.ScopeApi,
                OpenIddictConstants.Permissions.Prefixes.Scope + "custom",
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,

                // Response types
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.ResponseTypes.IdToken
            }
        };

        return descriptor;
    }
}
