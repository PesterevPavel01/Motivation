using Microsoft.Extensions.Options;
using Motivation.IdentityServer.Domain.Base;
using Motivation.IdentityServer.Domain.Configuration;
using Motivation.IdentityServer.Domain.Interfaces;
using Motivation.IdentityServer.Infrastructure.Base;
using OpenIddict.Abstractions;
using System.Text.Json;

namespace Motivation.IdentityServer.Infrastructure.Configurations.IdentityServerClients;

public class MachineToMachineClientConfiguration : ClientConfigurationBase, IIdentityClientConfiguration<OpenIddictApplicationDescriptor>
{
    private readonly AuthServerConfiguration _authConfig;

    public MachineToMachineClientConfiguration(IOptions<AuthServerConfiguration> authConfig)
    {
        _authConfig = authConfig.Value;
    }

    public OpenIddictApplicationDescriptor GetDescriptor()
    {
        if (!_authConfig.Clients.TryGetValue("MachineToMachine", out var clientConfig))
        {
            throw new InvalidOperationException("MachineToMachine client is not configured");
        }

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = clientConfig.ClientId,
            ClientSecret = clientConfig.ClientSecret,
            DisplayName = "Machine-To-Machine",
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                OpenIddictConstants.Permissions.GrantTypes.Password,
                OpenIddictConstants.Permissions.Prefixes.Scope + AppData.ScopeApi,
            },
            Properties =
            {
                ["roles"] = JsonSerializer.SerializeToElement(Array.Empty<string>())
            }
        };

        return descriptor;
    }
}
