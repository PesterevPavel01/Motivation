using Motivation.Contracts.Interfaces.QueueEvents;

namespace Motivation.Contracts.QueueEvents;

public sealed record SourceDataSyncedEvent(DateTime DateStart, DateTime DateEnd) : IKpiCalculationServiceQueueEvent;
