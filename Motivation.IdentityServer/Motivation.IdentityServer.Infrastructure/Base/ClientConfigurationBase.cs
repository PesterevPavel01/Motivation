namespace Motivation.IdentityServer.Infrastructure.Base
{
    public abstract class ClientConfigurationBase
    {
        public string ClientId { get; protected set; } = null!;
        public string ClientSecret { get; protected set; } = null!;
        public string[] Roles { get; protected set; } = [];
        public string[] Scopes { get; protected set; } = [];
    }
}
