using Motivation.Domain.Interfaces;

namespace Motivation.Domain.DomainEvents.EmployeeDomainEvents;

public sealed record PositionDeletedDomainEvent(Guid Id) : IDomainEvent;