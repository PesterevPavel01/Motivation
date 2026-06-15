using Calabonga.OperationResults;
using Motivation.Domain.DomainEvents.KpiDomainEvents;
using Motivation.Domain.DomainEvents.PositionDomainEvents;
using Motivation.Domain.Entities.Base;
using Motivation.Domain.Entities.Kpis;
using Motivation.Domain.Enum;
using Motivation.Domain.Events.PositionDomainEvents;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public class Position : AggregateRoot
{
    private readonly List<EmployeePosition> _employeePositions = [];
    private readonly List<PositionKpi> _positionKpiHistory = [];

    private Position(TitleValue title, CodeValue code, Guid id, SalaryValue baseSalary, WorkWeekType workWeekType) : base(id, code)
    {
        BaseSalary = baseSalary;
        WorkWeekType = workWeekType;
        Title = title;
    }

    public SalaryValue BaseSalary { get; private set; }
    public WorkWeekType WorkWeekType { get; private set; }
    public MotivationPart MotivationPart { get; private set; } = null!;
    public TitleValue Title { get; protected set; }
    public IReadOnlyCollection<EmployeePosition> EmployeePositions => _employeePositions.AsReadOnly();
    public IReadOnlyCollection<PositionKpi> PositionKpiHistory => _positionKpiHistory.AsReadOnly();

    public static Operation<Position, string> Create(TitleValue title, CodeValue code, SalaryValue baseSalary, WorkWeekType workWeekType, MotivationPart motivationPart)
    {
        if (code is null)
            return Operation.Error("Code is null!");
        if (title is null)
            return Operation.Error("Title is null!");
        if (baseSalary is null)
            return Operation.Error("Base salary is null!");

        var position = new Position(title, code, Guid.CreateVersion7(), baseSalary, workWeekType);

        position.SetMotivationPart(motivationPart);

        position.RaiseDomainEvent(new PositionCreatedDomainEvent(position.Id));

        return position;
    }

    private Operation<bool, string> SetMotivationPart(MotivationPart motivationPart) 
    {
        if (motivationPart is null)
            return Operation.Error("Motivation part is null!");
        MotivationPart = motivationPart;
        return true;
    }

    public Operation<bool, string> AssignKpi(PositionKpi positionKpi)
    {
        if (positionKpi is null)
            return Operation.Error("PositionKpi is null!");
        _positionKpiHistory.Add(positionKpi);

        RaiseDomainEvent(new AssignKpiToPositionDomainEvent(Id, positionKpi.Id));
        
        return true;
    }

    public Operation<Position, string> Update(
        TitleValue title, 
        CodeValue code, 
        SalaryValue baseSalary, 
        WorkWeekType workWeekType, 
        MotivationPart motivationPart)
    {
        if (code is null)
            return Operation.Error("Code is null!");
        if (title is null)
            return Operation.Error("Title is null!");
        if (baseSalary is null)
            return Operation.Error("Base salary is null!");

        BaseSalary = baseSalary;
        WorkWeekType = workWeekType;
        Title = title;
        Code = code;

        SetMotivationPart(motivationPart);

        RaiseDomainEvent(new PositionUpdatedDomainEvent(Id));

        return this;
    }
}