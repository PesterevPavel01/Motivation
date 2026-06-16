using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public sealed class EmployeePosition : Auditable
{
    private readonly List<ActualHours> _actualHoursHistory = [];
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
    public IReadOnlyCollection<ActualHours> ActualHoursHistory => _actualHoursHistory.AsReadOnly();

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

    public Operation<bool, string> RemoveExtraPart(CodeValue extraPartCode)
    {
        if (extraPartCode is null)
            return Operation.Error("Extra part code is null!");
        var existingExtraPart = _extraPartHistory.FirstOrDefault(x => x.Code == extraPartCode);
        if (existingExtraPart is null)
            return Operation.Error("Extra part not found!");
        _extraPartHistory.Remove(existingExtraPart);
        return true;
    }

    public Operation<bool, string> SetDeduction(Deduction deduction)
    {
        if (deduction is null)
            return Operation.Error("Deduction is null!");
        _deductionHistory.Add(deduction);
        return true;
    }

    public Operation<bool, string> RemoveDeduction(CodeValue deductionCode)
    {
        if (deductionCode is null)
            return Operation.Error("Deduction code is null!");
        var existingDeduction = _deductionHistory.FirstOrDefault(x => x.Code == deductionCode);
        if (existingDeduction is null)
            return Operation.Error("Deduction not found!");
        _deductionHistory.Remove(existingDeduction);
        return true;
    }

    public Operation<bool, string> SetActualHours(ActualHours hours)
    {
        if (hours is null)
            return Operation.Error("Hours is null!");

        var existingIndex = _actualHoursHistory
            .FindIndex(x => x.Year == hours.Year
                && x.Month == hours.Month);

        if (existingIndex >= 0)
        {
            _actualHoursHistory[existingIndex] = hours;
        }
        else
        {
            _actualHoursHistory.Add(hours);
        }
        return true;
    }

    internal Operation<bool, string> RemoveActualHours(MonthValue month, YearValue year)
    {
        if (month is null)
            return Operation.Error("Month is null!");
        if (year is null)
            return Operation.Error("Year is null!");

        var existingIndex = _actualHoursHistory
            .FindIndex(x => x.Year == year
                && x.Month == month);

        if (existingIndex >= 0)
        {
            _actualHoursHistory.RemoveAt(existingIndex);
        }
        else
        {
            return Operation.Error("ActualHours not found!");
        }
        return true;
    }
}