using Motivation.Domain.Interfaces;

namespace Motivation.Domain.DomainEvents.EmployeeDomainEvents;

public sealed record RemoveExtraPartDomainEvent(Guid EmployeeId, Guid PositionId) : IDomainEvent;
