using Motivation.IdentityServer.Domain.Interfaces;
using OpenIddict.Abstractions;

namespace Motivation.IdentityServer.Infrastructure;

public sealed class OpenIddictClientProcessor : IIdentityClientProcessor
{
    private readonly IOpenIddictApplicationManager _manager;
    private readonly IEnumerable<IIdentityClientConfiguration<OpenIddictApplicationDescriptor>> _clientConfigurations;

    public OpenIddictClientProcessor(
        IEnumerable<IIdentityClientConfiguration<OpenIddictApplicationDescriptor>> clientConfigurations,
        IOpenIddictApplicationManager manager)
    {
        _clientConfigurations = clientConfigurations;
        _manager = manager;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        foreach (var configuration in _clientConfigurations)
        {
            var descriptor = configuration.GetDescriptor();
            var existingClient = await _manager.FindByClientIdAsync(descriptor.ClientId, cancellationToken);
                
            //await _manager.DeleteAsync(existingClient, cancellationToken);
            //await _manager.CreateAsync(descriptor, cancellationToken);

            if (existingClient is null)
            {
                await _manager.CreateAsync(descriptor, cancellationToken);
            }
        }
    }
}
