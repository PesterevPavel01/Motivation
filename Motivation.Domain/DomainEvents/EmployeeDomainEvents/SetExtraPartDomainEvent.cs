using Motivation.Domain.Interfaces;

namespace Motivation.Domain.DomainEvents.EmployeeDomainEvents;

public sealed record SetExtraPartDomainEvent(Guid EmployeeId, Guid PositionId, Guid ExtraPartId) : IDomainEvent;