using Motivation.Domain.Interfaces;

namespace Motivation.Domain.DomainEvents.KpiDomainEvents;

public record AssignKpiToPositionDomainEvent(Guid PositionKpiId, Guid KpiId) : IDomainEvent;
