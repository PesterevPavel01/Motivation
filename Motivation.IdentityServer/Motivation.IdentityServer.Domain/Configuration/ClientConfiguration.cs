namespace Motivation.IdentityServer.Domain.Configuration;

public sealed class ClientConfiguration
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public List<string> ClientRoles { get; set; } = [];
}
