namespace Motivation.IdentityServer.Domain.Configuration;


public sealed class AuthServerConfiguration
{
    public required string Url { get; set; }
    public bool DevCertificates { get; set; }
    public Dictionary<string, ClientConfiguration> Clients { get; set; } = [];
}
