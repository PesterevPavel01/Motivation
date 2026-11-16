using Mediator;
using Motivation.Contracts.Dto.CalculateDto;

namespace Motivation.Contracts.QueueEvents;

public sealed record RecalculateKpiRequest(KpiCalculationTask Model) : IRequest<RecalculateKpiResponse>;