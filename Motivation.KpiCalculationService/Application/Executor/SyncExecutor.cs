using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.KpiCalculationService.Application.Interfaces;

namespace Motivation.KpiCalculationService.Application.Executor;

public sealed class SyncExecutor : IExecutor<SyncDto>
{
    private readonly IEnumerable<ISyncStep> _syncSteps = [];

    public SyncExecutor(IEnumerable<ISyncStep> steps)
    {
        _syncSteps = steps.OrderBy(x => x.OrderIndex);
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
        return true;
    }
}
