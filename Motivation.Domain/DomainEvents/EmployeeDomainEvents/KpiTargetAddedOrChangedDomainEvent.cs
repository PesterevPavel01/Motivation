using Motivation.Domain.Interfaces;

namespace Motivation.Domain.DomainEvents.EmployeeDomainEvents;

public sealed record KpiTargetAddedOrChangedDomainEvent(Guid TargetId) : IDomainEvent;
