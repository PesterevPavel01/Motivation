using Mediator;
using Motivation.Domain.DomainEvents.EmployeeDomainEvents;
using Motivation.Domain.DomainEvents.KpiDomainEvents;
using Motivation.Domain.DomainEvents.PositionDomainEvents;
using Motivation.Domain.Events.EmployeeDomainEvents;
using Motivation.Domain.Events.KpiDomainEvents;
using Motivation.Domain.Events.PositionDomainEvents;
using System.Text.Json.Serialization;

namespace Motivation.Domain.Interfaces;

[JsonDerivedType(typeof(PositionCreatedDomainEvent), typeDiscriminator: "position-created")]
[JsonDerivedType(typeof(PositionUpdatedDomainEvent), typeDiscriminator: "position-updated")]
[JsonDerivedType(typeof(PositionDeletedDomainEvent), typeDiscriminator: "position-deleted")]
[JsonDerivedType(typeof(KpiCreatedDomainEvent), typeDiscriminator: "kpi-created")]
[JsonDerivedType(typeof(KpiDeletedDomainEvent), typeDiscriminator: "kpi-deleted")]
[JsonDerivedType(typeof(KpiUpdatedDomainEvent), typeDiscriminator: "kpi-updated")]
[JsonDerivedType(typeof(EmployeeCreatedDomainEvent), typeDiscriminator: "employee-created")]
[JsonDerivedType(typeof(EmployeeDeletedDomainEvent), typeDiscriminator: "employee-deleted")]
[JsonDerivedType(typeof(EmployeeUpdatedDomainEvent), typeDiscriminator: "employee-updated")]
[JsonDerivedType(typeof(AssignEmployeeToPositionDomainEvent), typeDiscriminator: "assign-to-position")]
[JsonDerivedType(typeof(DismissFromPositionDomainEvent), typeDiscriminator: "dismiss-from-position")]
[JsonDerivedType(typeof(AssignKpiToPositionDomainEvent), typeDiscriminator: "assign-kpi")]
[JsonDerivedType(typeof(KpiRemovedFromPositionDomainEvent), typeDiscriminator: "remove-kpi-from-position")]
[JsonDerivedType(typeof(SetActualHoursDomainEvent), typeDiscriminator: "set-actual-hours")]
[JsonDerivedType(typeof(RemoveActualHoursDomainEvent), typeDiscriminator: "delete-actual-hours")]
[JsonDerivedType(typeof(SetDeductionDomainEvent), typeDiscriminator: "set-deduction")]
[JsonDerivedType(typeof(RemoveDeductionDomainEvent), typeDiscriminator: "remove-deduction")]
[JsonDerivedType(typeof(SetExtraPartDomainEvent), typeDiscriminator: "set-extra-part")]
[JsonDerivedType(typeof(RemoveExtraPartDomainEvent), typeDiscriminator: "remove-extra-part")]

public interface IDomainEvent : INotification { }
