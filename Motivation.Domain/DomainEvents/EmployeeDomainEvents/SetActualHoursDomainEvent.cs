using Motivation.Domain.Interfaces;

namespace Motivation.Domain.DomainEvents.EmployeeDomainEvents;

public sealed record SetActualHoursDomainEvent(Guid EmployeeId, Guid PositionId, int Month, int Year) : IDomainEvent;
