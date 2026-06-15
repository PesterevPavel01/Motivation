using Mediator;
using Motivation.Domain.DomainEvents.KpiDomainEvents;
using Motivation.Domain.DomainEvents.PositionDomainEvents;
using Motivation.Domain.Events.EmployeeDomainEvents;
using Motivation.Domain.Events.KpiDomainEvents;
using Motivation.Domain.Events.PositionDomainEvents;
using System.Text.Json.Serialization;

namespace Motivation.Domain.Interfaces;

[JsonDerivedType(typeof(PositionCreatedDomainEvent), typeDiscriminator: "position-created")]
[JsonDerivedType(typeof(PositionUpdatedDomainEvent), typeDiscriminator: "position-updated")]
[JsonDerivedType(typeof(KpiCreatedDomainEvent), typeDiscriminator: "kpi-created")]
[JsonDerivedType(typeof(KpiUpdatedDomainEvent), typeDiscriminator: "kpi-updated")]
[JsonDerivedType(typeof(EmployeeCreatedDomainEvent), typeDiscriminator: "employee-created")]
[JsonDerivedType(typeof(AssignEmployeeToPositionDomainEvent), typeDiscriminator: "assign-to-position")]
[JsonDerivedType(typeof(AssignKpiToPositionDomainEvent), typeDiscriminator: "assign-kpi")]

public interface IDomainEvent : INotification { }
