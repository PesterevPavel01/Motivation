using Motivation.Domain.Interfaces;

namespace Motivation.Domain.DomainEvents.KpiDomainEvents;

public sealed record KpiDeletedDomainEvent(Guid Id) : IDomainEvent;
