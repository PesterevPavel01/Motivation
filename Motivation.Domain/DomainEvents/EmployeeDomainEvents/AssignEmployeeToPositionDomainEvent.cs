using Motivation.Domain.Interfaces;

namespace Motivation.Domain.Events.EmployeeDomainEvents;

public sealed record AssignEmployeeToPositionDomainEvent(Guid EmployeeId, Guid PositionId) : IDomainEvent;
