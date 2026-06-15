using Motivation.Domain.Interfaces;

namespace Motivation.Domain.DomainEvents.PositionDomainEvents;

public sealed record PositionUpdatedDomainEvent(Guid Id) : IDomainEvent;
