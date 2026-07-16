using Motivation.Contracts.Interfaces.QueueEvents;

namespace Motivation.Contracts.QueueEvents;

public sealed record SourceDataSyncedQueueEvent(DateTime DateStart, DateTime DateEnd) : ISyncServiceQueueEvent;
