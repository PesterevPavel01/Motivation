using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;

namespace Motivation.KpiCalculationService.Application.Interfaces;

public interface IExecutor<TDto>
    where TDto : class
{
    Task<Operation<bool, string>> ExecuteAsync(TDto model, CancellationToken cancellationToken);
}
