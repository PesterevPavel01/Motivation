using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Contracts.Interfaces.QueueEvents;

namespace Motivation.Contracts.QueueEvents;

public  sealed record KpiValueCalculatedQueueEvent(KpiCalculationTaskResult Result) : IKpiCalculationServiceQueueEvent;
