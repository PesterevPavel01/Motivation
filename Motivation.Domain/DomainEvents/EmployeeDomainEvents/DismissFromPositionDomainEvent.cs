using Motivation.Domain.Interfaces;

namespace Motivation.Domain.DomainEvents.EmployeeDomainEvents;

public sealed record DismissFromPositionDomainEvent(Guid EmployeeId, Guid PositionId) : IDomainEvent;
