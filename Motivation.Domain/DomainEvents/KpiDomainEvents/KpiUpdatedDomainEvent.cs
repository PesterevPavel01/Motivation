using Motivation.Domain.Interfaces;

namespace Motivation.Domain.Events.KpiDomainEvents;

public sealed record KpiUpdatedDomainEvent(Guid KpiId) : IDomainEvent;
