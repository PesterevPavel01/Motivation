using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.Entities;

public sealed class EmployeePosition : Auditable
{
    private readonly List<ExtraPart> _extraPartHistory = [];
    private readonly List<Deduction> _deductionHistory = [];
    private EmployeePosition(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
    public DateTime AssignmentDate { get; private set; }

    public Employee Employee { get; private set; } = null!;
    public Guid EmployeeId { get; private set; }
    public Position Position { get; private set; } = null!;
    public Guid PositionId { get; private set; }

    public IReadOnlyCollection<ExtraPart> ExtraPartHistory => _extraPartHistory.AsReadOnly();
    public IReadOnlyCollection<Deduction> DeductionHistory => _deductionHistory.AsReadOnly();

    public ExtraPart LastExtraPart => _extraPartHistory.Last();
    public Deduction LastDeduction => _deductionHistory.Last();

    public static Operation<EmployeePosition, string> Create(Position position, Employee employee, DateTime assignmentDate) 
    {
        if (position is null)
            return Operation.Error("Position is null!");
        if (employee is null)
            return Operation.Error("Employee is null!");
        if (assignmentDate == default)
            return Operation.Error("AssignmentDate is default DateTime!");

        var employeePosition = new EmployeePosition(Guid.CreateVersion7())
        {
            Employee = employee,
            Position = position,
            AssignmentDate = assignmentDate
        };

        return employeePosition;
    }

    public Operation<bool, string> SetExtraPart(ExtraPart extraPart) 
    {
        if (extraPart is null)
            return Operation.Error("Extra part is null!");
        _extraPartHistory.Add(extraPart);
        return true;
    }

    public Operation<bool, string> SetDeduction(Deduction deduction)
    {
        if (deduction is null)
            return Operation.Error("Deduction is null!");
        _deductionHistory.Add(deduction);
        return true;
    }
}