namespace Motivation.Contracts.Interfaces;

public interface IOutboxProcessor
{
    Task ProcessAsync(CancellationToken cancellationToken);
}

public interface IOutboxCleanerProcessor 
{
    Task ProcessAsync(CancellationToken cancellationToken);
}
