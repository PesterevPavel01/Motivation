using Motivation.Domain.Interfaces;

namespace Motivation.Domain.DomainEvents.EmployeeDomainEvents;

public sealed record SetDeductionDomainEvent(Guid EmployeeId, Guid PositionId, Guid DeductionId) : IDomainEvent;