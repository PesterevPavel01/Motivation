namespace Motivation.IdentityServer.Domain.Interfaces;

public interface IIdentityClientProcessor
{
    Task ProcessAsync(CancellationToken cancellationToken);
}
