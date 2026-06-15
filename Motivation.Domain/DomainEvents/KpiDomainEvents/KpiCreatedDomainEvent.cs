using Motivation.Domain.Interfaces;

namespace Motivation.Domain.Events.KpiDomainEvents;

public sealed record KpiCreatedDomainEvent(Guid KpiId) : IDomainEvent;
