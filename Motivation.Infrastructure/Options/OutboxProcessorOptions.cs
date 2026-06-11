namespace Motivation.Infrastructure.Options;

public class OutboxProcessorOptions
{
    public IEnumerable<string> SupportedEventTypes { get; set; } = [];
}