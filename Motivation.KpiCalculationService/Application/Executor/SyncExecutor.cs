using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.QueueEvents;
using Motivation.KpiCalculationService.Application.Interfaces;
using Rebus.Bus;

namespace Motivation.KpiCalculationService.Application.Executor;

public sealed class SyncExecutor : IExecutor<SyncDto>
{
    private readonly IEnumerable<ISyncStep> _syncSteps = [];
    private readonly IBus _bus;

    public SyncExecutor(IEnumerable<ISyncStep> steps, IBus bus)
    {
        _syncSteps = steps.OrderBy(x => x.OrderIndex);
        _bus = bus;
    }

    public async Task<Operation<bool, string>> ExecuteAsync(SyncDto model, CancellationToken cancellationToken)
    {
        var dateStart = model.DateStart.ToUniversalTime();
        var dateEnd = model.DateEnd.ToUniversalTime();

        foreach (var syncStep in _syncSteps)
        {
            var result = await syncStep.ExecuteAsync(model, dateStart, dateEnd, cancellationToken);

            if (!result.Ok)
            {
                return Operation.Error($"Step {syncStep.OrderIndex} failed: {result.Error}");
            }
        }
        await _bus.Publish(new SourceDataSyncedEvent(dateStart, dateEnd));
        return true;
    }
}
