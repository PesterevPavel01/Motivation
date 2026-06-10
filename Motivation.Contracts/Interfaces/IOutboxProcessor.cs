namespace Motivation.Contracts.Interfaces;

public interface IOutboxProcessor
{
    Task ProcessAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
}

public interface IOutboxCleanerProcessor 
{
    Task ProcessAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
}
