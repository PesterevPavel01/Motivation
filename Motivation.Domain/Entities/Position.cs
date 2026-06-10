using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public class Position : SimpleEntity
{
    private readonly List<EmployeePosition> _employeePositions = [];
    private readonly List<PositionKpi> _positionKpiHistory = [];

    private Position(TitleValue title, CodeValue code, Guid id, SalaryValue baseSalary, WorkWeekType workWeekType) : base(title, code, id)
    {
        BaseSalary = baseSalary;
        WorkWeekType = workWeekType;
    }

    public SalaryValue BaseSalary { get; private set; }
    public WorkWeekType WorkWeekType { get; private set; }
    public MotivationPart MotivationPart { get; private set; } = null!;

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

        return position;
    }

    public Operation<bool, string> SetMotivationPart(MotivationPart motivationPart) 
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
        return true;
    }
}