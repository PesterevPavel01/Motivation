using Calabonga.OperationResults;
using Motivation.Contracts.Dto.CalculateDto;

namespace Motivation.Contracts.QueueEvents;

public record RecalculateKpiResponse(KpiCalculationTaskResult? Result, string? Error);
