using Motivation.Domain.Interfaces;

namespace Motivation.Domain.Events.EmployeeDomainEvents;

public sealed record EmployeeCreatedDomainEvent(Guid Id) : IDomainEvent;