using OpenIddict.Abstractions;

namespace Motivation.IdentityServer.Infrastructure.Configurations.Clients
{
    public interface IOpenIddictClientConfiguration
    {
        OpenIddictApplicationDescriptor GetDescriptor();
    }
}
