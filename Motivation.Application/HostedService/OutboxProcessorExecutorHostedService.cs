using Calabonga.Microservices.BackgroundWorkers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Motivation.Contracts.Interfaces;
using System.Reflection;

namespace Motivation.Application.HostedService;
public sealed class OutboxProcessorExecutorHostedService : ScheduledHostedServiceBase
{
    private readonly string _serviceName;
    public OutboxProcessorExecutorHostedService(IServiceScopeFactory serviceScopeFactory, ILogger<OutboxCleanerProcessorExecutorHostedService> logger)
        : base(serviceScopeFactory, logger)
    {
        _serviceName = Assembly.GetEntryAssembly()?.GetName().Name
            ?? AppDomain.CurrentDomain.FriendlyName;
    }

    protected override async Task ProcessInScopeAsync(IServiceProvider serviceProvider, CancellationToken token)
    {
        var outboxProcessor = serviceProvider.GetRequiredService<IOutboxProcessor>();
        await outboxProcessor.ProcessAsync(token);
    }

    protected override string Schedule => "0/30 * * * * *"; // every 30 seconds

    protected override bool IncludingSeconds => true;

    protected override string DisplayName => $"{_serviceName}.OutboxProcessorExecutorHostedService";

    #if DEBUG
        protected override bool IsExecuteOnServerRestart => true;
    #else
        protected override bool IsExecuteOnServerRestart => false;
    #endif
}

public sealed class OutboxCleanerProcessorExecutorHostedService : ScheduledHostedServiceBase
{
    private readonly string _serviceName;
    public OutboxCleanerProcessorExecutorHostedService(IServiceScopeFactory serviceScopeFactory, ILogger<OutboxCleanerProcessorExecutorHostedService> logger)
        : base(serviceScopeFactory, logger)
    {
        _serviceName = Assembly.GetEntryAssembly()?.GetName().Name
            ?? AppDomain.CurrentDomain.FriendlyName;
    }

    protected override async Task ProcessInScopeAsync(IServiceProvider serviceProvider, CancellationToken token)
    {
        var outboxProcessor = serviceProvider.GetRequiredService<IOutboxCleanerProcessor>();
        await outboxProcessor.ProcessAsync(token);
    }

    protected override string Schedule => "0 0 0 */10 * *"; // каждые 10 дней в полночь

    protected override bool IncludingSeconds => true;

    protected override string DisplayName => $"{_serviceName}.OutboxCleanerProcessorExecutorHostedService";

#if DEBUG
    protected override bool IsExecuteOnServerRestart => true;
#else
        protected override bool IsExecuteOnServerRestart => false;
#endif
}