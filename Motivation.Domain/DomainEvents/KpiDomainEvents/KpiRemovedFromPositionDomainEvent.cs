using Motivation.Domain.Interfaces;

namespace Motivation.Domain.DomainEvents.KpiDomainEvents;

public record KpiRemovedFromPositionDomainEvent(Guid PositionKpiId, Guid KpiId) : IDomainEvent;
