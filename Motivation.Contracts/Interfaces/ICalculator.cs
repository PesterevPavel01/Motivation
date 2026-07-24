using Calabonga.OperationResults;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Domain.Enum;

namespace Motivation.Contracts.Interfaces;

public interface ICalculator
{
    abstract KpiType KpiType { get; }
    Task<Operation<KpiCalculationTaskResult, string>> CalculateKpiAsync(
        KpiCalculationTask task,
        CancellationToken cancellationToken = default);
}
