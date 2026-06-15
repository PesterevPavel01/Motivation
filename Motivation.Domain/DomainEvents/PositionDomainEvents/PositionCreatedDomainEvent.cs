using Motivation.Domain.Interfaces;

namespace Motivation.Domain.Events.PositionDomainEvents;

public sealed record PositionCreatedDomainEvent(Guid PositionId) : IDomainEvent;