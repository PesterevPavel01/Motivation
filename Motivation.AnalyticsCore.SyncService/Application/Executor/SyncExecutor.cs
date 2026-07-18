using Calabonga.OperationResults;
using Motivation.AnaliticsCore.SyncService.Application.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.QueueEvents;
using Rebus.Bus;
using System.Data;

namespace Motivation.AnaliticsCore.SyncService.Application.Executor
{
    public sealed class SyncExecutor : IExecutor<SyncDto>
    {
        private readonly IEnumerable<ISyncStep> _syncSteps = [];
        private readonly IBus _bus;
        private readonly ILogger<SyncExecutor> _logger;

        public SyncExecutor(IEnumerable<ISyncStep> steps, IBus bus, ILogger<SyncExecutor> logger)
        {
            _syncSteps = steps.OrderBy(x => x.OrderIndex);
            _bus = bus;
            _logger = logger;
        }

        public async Task<Operation<bool, string>> ExecuteAsync(SyncDto model, CancellationToken cancellationToken)
        {
            foreach (var syncStep in _syncSteps)
            {
                var result = await syncStep.ExecuteAsync(model, model.DateStart, model.DateEnd, cancellationToken);

                if (!result.Ok)
                {
                    return Operation.Error($"Step {syncStep.OrderIndex} failed: {result.Error}");
                }
            }
            await _bus.Publish(new SourceDataSyncedQueueEvent(model.DateStart, model.DateEnd));
            _logger.LogInformation("Message  {@SourceDataSyncedQueueEvent} sent!", typeof(SourceDataSyncedQueueEvent).Name.ToString());
            return true;
        }
    }
}
