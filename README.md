markdown
---

```mermaid
classDiagram
    class Identity {
        <<abstract>>
        +Guid Id
    }

    class IAuditable {
        <<interface>>
        +DateTime CreatedAt
        +DateTime UpdatedAt
    }

    class Auditable {
        +DateTime CreatedAt
        +DateTime UpdatedAt
    }

    class Entity {
        <<abstract>>
        +Guid Id
        +CodeValue Code
        +bool Enabled
        +Disable()
        +Equals()
        +GetHashCode()
    }

    class SimpleEntity {
        <<abstract>>
        +TitleValue Title
    }

    class AggregateRoot {
        <<abstract>>
        -List~IDomainEvent~ _domainEvents
        +GetDomainEvents()
        +ClearDomainEvents()
        #RaiseDomainEvent()
    }

    class Deduction {
        +MonthValue Month
        +YearValue Year
        +DeductionValue DeductionValue
        +EmployeePosition EmployeePosition
        +Create()
    }

    class Employee {
        +FirstNameValue FirstName
        +LastNameValue LastName
        +SecondNameValue? SecondName
        +FullNameValue FullName
        +IReadOnlyCollection~EmployeePosition~ EmployeePositions
        +Create()
        +AssignToPosition()
    }

    class EmployeePosition {
        +Guid Id
        +DateTime AssignmentDate
        +Employee Employee
        +Guid EmployeeId
        +Position Position
        +Guid PositionId
        +IReadOnlyCollection~ExtraPart~ ExtraPartHistory
        +IReadOnlyCollection~Deduction~ DeductionHistory
        +ExtraPart LastExtraPart
        +Deduction LastDeduction
        +Create()
        +SetExtraPart()
        +SetDeduction()
    }

    class ExtraPart {
        +ExtraPartValue ExtraPartValue
        +DateTime ValidFrom
        +DateTime ValidTo
        +EmployeePosition EmployeePosition
        +Create()
        +SetValidTo()
    }

    class Kpi {
        +KpiType Type
        +AbbreviationValue Abbreviation
        +CalculationType CalculationType
        +MeasurementUnit MeasurementUnit
        +IReadOnlyList~KpiFilter~ Filters
        +Create()
        +AddFilter()
    }

    class KpiFilter {
        <<abstract>>
        +Guid Id
        +TitleValue Title
        +KpiFilterType KpiFilterType
    }

    class KpiFilterCompany {
        +Create()
        +KpiFilterType KpiFilterType
    }

    class KpiFilterCustomer {
        +Create()
        +KpiFilterType KpiFilterType
    }

    class KpiFilterProduct {
        +Create()
        +KpiFilterType KpiFilterType
    }

    class MeasurementUnit {
        +IReadOnlyCollection~Kpi~ Kpis
        +Create()
    }

    class MotivationPart {
        +MotivationPartValue MotivationPartValue
        +bool RecalculateToHours
        +Create()
        +SetRecalculateToHours()
    }

    class Position {
        +SalaryValue BaseSalary
        +WorkWeekType WorkWeekType
        +MotivationPart MotivationPart
        +IReadOnlyCollection~EmployeePosition~ EmployeePositions
        +IReadOnlyCollection~PositionKpi~ PositionKpiHistory
        +Create()
        +SetMotivationPart()
        +AssignKpi()
    }

    class PositionKpi {
        +Guid Id
        +DateTime ValidFrom
        +DateTime ValidTo
        +int OrderNumber
        +WeightValue Weight
        +Position Position
        +Kpi Kpi
        +IReadOnlyCollection~KpiTarget~ TargetHistory
        +Create()
        +AddTarget()
    }

    class KpiTarget {
        +Guid Id
        +DateTime ValidFrom
        +DateTime ValidTo
        +TargetValue Target
        +KpiFact? KpiFact
        +PositionKpi PositionKpi
        +Create()
        +SetFact()
    }

    class KpiFact {
        +FactValue Fact
        +AchievementValue Achievement
        +BonusAmountValue BonusAmount
        +Create()
    }

    class StandardHours {
        +WorkWeekType WorkWeekType
        +Guid Id
        +MonthValue Month
        +YearValue Year
        +StandardHoursValue StandardHoursValue
        +Create()
    }

    class EventItem {
        +DateTime CreatedAt
        +string Logger
        +string Level
        +string Message
        +string? ThreadId
        +string? ExceptionMessage
    }

    Identity <|-- EventItem

    IAuditable <|.. Auditable
    Auditable <|-- Entity
    Entity <|-- SimpleEntity
    SimpleEntity <|-- AggregateRoot
    SimpleEntity <|-- Deduction
    SimpleEntity <|-- ExtraPart
    SimpleEntity <|-- Kpi
    SimpleEntity <|-- MeasurementUnit
    SimpleEntity <|-- Position
    Auditable <|-- EmployeePosition
    Auditable <|-- KpiFilter
    Auditable <|-- PositionKpi
    Auditable <|-- StandardHours

    KpiFilter <|-- KpiFilterCompany
    KpiFilter <|-- KpiFilterCustomer
    KpiFilter <|-- KpiFilterProduct

    EmployeePosition --> Employee
    EmployeePosition --> Position
    EmployeePosition --> ExtraPart
    EmployeePosition --> Deduction

    Employee --> EmployeePosition
    Position --> EmployeePosition
    Position --> MotivationPart
    Position --> PositionKpi

    PositionKpi --> Kpi
    PositionKpi "1" --> "*" KpiTarget

    KpiTarget --> KpiFact
    Kpi --> MeasurementUnit
    Kpi --> KpiFilter

    AggregateRoot <|-- Position
    AggregateRoot <|-- Employee

```
---
