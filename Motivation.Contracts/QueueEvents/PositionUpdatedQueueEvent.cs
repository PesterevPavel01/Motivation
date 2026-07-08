using Motivation.Contracts.Interfaces.Events;

namespace Motivation.Contracts.Events;

public sealed record PositionUpdatedQueueEvent(Guid Id) : IHrServiceQueueEvent;
