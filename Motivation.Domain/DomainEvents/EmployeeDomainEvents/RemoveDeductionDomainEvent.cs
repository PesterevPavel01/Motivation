using Motivation.Domain.Interfaces;

namespace Motivation.Domain.DomainEvents.EmployeeDomainEvents;

public sealed record RemoveDeductionDomainEvent(Guid EmployeeId, Guid PositionId) : IDomainEvent;
