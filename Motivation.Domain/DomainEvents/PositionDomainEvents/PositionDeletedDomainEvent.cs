using Motivation.Domain.Interfaces;

namespace Motivation.Domain.DomainEvents.PositionDomainEvents;

public sealed record PositionDeletedDomainEvent(Guid Id) : IDomainEvent;