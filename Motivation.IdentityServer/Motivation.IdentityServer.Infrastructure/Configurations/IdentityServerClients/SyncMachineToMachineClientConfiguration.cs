using Microsoft.Extensions.Options;
using Motivation.IdentityServer.Domain.Base;
using Motivation.IdentityServer.Domain.Configuration;
using Motivation.IdentityServer.Domain.Interfaces;
using OpenIddict.Abstractions;
using System.Text.Json;

namespace Motivation.IdentityServer.Infrastructure.Configurations.IdentityServerClients;

public class SyncMachineToMachineClientConfiguration : IIdentityClientConfiguration<OpenIddictApplicationDescriptor>
{
    private readonly AuthServerConfiguration _authConfig;

    public SyncMachineToMachineClientConfiguration(IOptions<AuthServerConfiguration> authConfig)
    {
        _authConfig = authConfig.Value;
    }

    public OpenIddictApplicationDescriptor GetDescriptor()
    {
        if (!_authConfig.Clients.TryGetValue("SyncMachineToMachine", out var clientConfig))
        {
            throw new InvalidOperationException("SyncMachineToMachine client is not configured");
        }

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = clientConfig.ClientId,
            ClientSecret = clientConfig.ClientSecret,
            DisplayName = "Sync-Machine-To-Machine", 
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                OpenIddictConstants.Permissions.GrantTypes.Password,
                OpenIddictConstants.Permissions.Prefixes.Scope + AppData.ScopeApi,
                OpenIddictConstants.Permissions.Prefixes.Scope + AppData.ScopeSync,
            },
            Properties =
            {
                ["roles"] = JsonSerializer.SerializeToElement<string[]>([AppData.SystemAdministratorRoleName])
            }
        };

        return descriptor;
    }
}
