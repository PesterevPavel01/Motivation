using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;

namespace Motivation.KpiCalculationService.Application.Interfaces;

public interface ISyncStep
{
    public abstract int OrderIndex { get; }
    Task<Operation<bool, string>> ExecuteAsync(
        SyncDto model,
        DateTime dateStart,
        DateTime dateEnd,
        CancellationToken cancellationToken);
}