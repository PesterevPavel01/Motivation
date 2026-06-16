using Calabonga.OperationResults;
using Motivation.Domain.DomainEvents.EmployeeDomainEvents;
using Motivation.Domain.Entities.Base;
using Motivation.Domain.Events.EmployeeDomainEvents;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public class Employee : AggregateRoot
{
    private readonly List<EmployeePosition> _employeePositions = [];

    private Employee(Guid id, CodeValue code, FirstNameValue firstName, LastNameValue lastName, SecondNameValue? secondName, FullNameValue fullName) : base(id, code) 
    {
        FirstName = firstName;
        LastName = lastName;
        SecondName = secondName;
        FullName = fullName;
    }

    public FirstNameValue FirstName { get; private set; } = null!;
    public LastNameValue LastName { get; private set;} = null!;
    public SecondNameValue? SecondName { get; private set; }
    public FullNameValue FullName { get; private set; }

    public IReadOnlyCollection<EmployeePosition> EmployeePositions => _employeePositions.AsReadOnly();

    public static Operation<Employee, string> Create(CodeValue code, FirstNameValue firstName, LastNameValue lastName, SecondNameValue? secondName, FullNameValue fullName)
    {
        if (code is null)
            return Operation.Error("Code is null!");
        if (firstName is null)
            return Operation.Error("First name is null!");
        if (lastName is null)
            return Operation.Error("Last name is null!");
        if (fullName is null)
            return Operation.Error("Full name is null!");
        var employee = new Employee(Guid.CreateVersion7(), code, firstName, lastName, secondName, fullName);

        employee.RaiseDomainEvent(new EmployeeCreatedDomainEvent(employee.Id));

        return employee;
    }

    public Operation<bool, string> Delete()
    {
        RaiseDomainEvent(new EmployeeDeletedDomainEvent(Id));

        return true;
    }

    public Operation<bool, string> Update(CodeValue code, FirstNameValue firstName, LastNameValue lastName, SecondNameValue? secondName, FullNameValue fullName)
    {
        if (code is null)
            return Operation.Error("Code is null!");
        if (firstName is null)
            return Operation.Error("First name is null!");
        if (lastName is null)
            return Operation.Error("Last name is null!");
        if (fullName is null)
            return Operation.Error("Full name is null!");
        Code = code;
        FirstName = firstName;
        LastName = lastName;
        SecondName = secondName;
        FullName = fullName;

        RaiseDomainEvent(new EmployeeUpdatedDomainEvent(Id));

        return true;
    }

    public Operation<bool, string> AssignToPosition(Position position, DateTime assignmentDate)
    {
        if (position is null)
            return Operation.Error("Position is null!");
        if (assignmentDate == default)
            return Operation.Error("AssignmentDate is default DateTime!");
        if(EmployeePositions is null)
            return Operation.Error("EmployeePositions is null!");
        if (EmployeePositions.Any(x => x.Position.Id == position.Id))
            return Operation.Error($"Employee {Id} is already assigned to position {position.Id}");

        var employeePosition = EmployeePosition.Create(position, this, assignmentDate);
        if (!employeePosition.Ok)
            return Operation.Error(employeePosition.Error);

        _employeePositions.Add(employeePosition.Result);

        RaiseDomainEvent(new AssignEmployeeToPositionDomainEvent(Id, position.Id));

        return true;
    }
    public Operation<bool, string> SetExtraPart(CodeValue positionCode, ExtraPart extraPart)
    {
        if (extraPart is null)
            return Operation.Error("Extra part is null!");
        var employeePosition = _employeePositions
            .FirstOrDefault(x => x.Position?.Code == positionCode);
        if(employeePosition is null)
            return Operation.Error("Employee position not found!");
        var setExtrsPart = employeePosition.SetExtraPart(extraPart);
        if (!setExtrsPart.Ok)
            return Operation.Error(setExtrsPart.Error);

        RaiseDomainEvent(new SetExtraPartDomainEvent(Id, employeePosition.Position.Id, extraPart.Id));

        return true;
    }

    public Operation<bool, string> RemoveExtraPart(CodeValue positionCode, CodeValue extraPartCode)
    {
        if (extraPartCode is null)
            return Operation.Error("Extra part code is null!");
        var employeePosition = _employeePositions
            .FirstOrDefault(x => x.Position?.Code == positionCode);
        if (employeePosition is null)
            return Operation.Error("Employee position not found!");
        var setExtrsPart = employeePosition.RemoveExtraPart(extraPartCode);
        if (!setExtrsPart.Ok)
            return Operation.Error(setExtrsPart.Error);

        RaiseDomainEvent(new RemoveExtraPartDomainEvent(Id, employeePosition.Position.Id));

        return true;
    }

    public Operation<bool, string> SetDeduction(CodeValue positionCode, Deduction deduction)
    {
        if (deduction is null)
            return Operation.Error("Deduction is null!");
        var employeePosition = _employeePositions
            .FirstOrDefault(x => x.Position?.Code == positionCode);
        if (employeePosition is null)
            return Operation.Error("Employee position not found!");
        var setExtrsPart = employeePosition.SetDeduction(deduction);
        if (!setExtrsPart.Ok)
            return Operation.Error(setExtrsPart.Error);

        RaiseDomainEvent(new SetDeductionDomainEvent(Id, employeePosition.Position.Id, deduction.Id));

        return true;
    }

    public Operation<bool, string> RemoveDeduction(CodeValue positionCode, CodeValue deductionCode)
    {
        if (deductionCode is null)
            return Operation.Error("Deduction code is null!");
        var employeePosition = _employeePositions
            .FirstOrDefault(x => x.Position?.Code == positionCode);
        if (employeePosition is null)
            return Operation.Error("Employee position not found!");
        var setDeduction = employeePosition.RemoveDeduction(deductionCode);
        if (!setDeduction.Ok)
            return Operation.Error(setDeduction.Error);

        RaiseDomainEvent(new RemoveDeductionDomainEvent(Id, employeePosition.Position.Id));
        
        return true;
    }

    public Operation<bool, string> SetActualHours(Position position, ActualHours hours)
    { 
        if(_employeePositions is null || !_employeePositions.Any())
            return Operation.Error("EmployeePositions is null or empty!");

        var currentEmployeePosition = _employeePositions
            .FirstOrDefault(x => x.Position.Id == position.Id);

        if(currentEmployeePosition is null)
            return Operation.Error("EmployeePosition not found!");

        var setHours = currentEmployeePosition.SetActualHours(hours);
        if(!setHours.Ok)
            return Operation.Error(setHours.Error);

        RaiseDomainEvent(new SetActualHoursDomainEvent(Id, position.Id, hours.Month.Value, hours.Year.Value));

        return true;
    }
    public Operation<bool, string> RemoveActualHours(Position position, MonthValue month, YearValue year)
    {
        if (_employeePositions is null || !_employeePositions.Any())
            return Operation.Error("EmployeePositions is null or empty!");

        var currentEmployeePosition = _employeePositions
            .FirstOrDefault(x => x.Position.Id == position.Id);

        if (currentEmployeePosition is null)
            return Operation.Error("EmployeePosition not found!");

        var deleteHours = currentEmployeePosition.RemoveActualHours(month, year);
        if (!deleteHours.Ok)
            return Operation.Error(deleteHours.Error);

        RaiseDomainEvent(new RemoveActualHoursDomainEvent(Id, position.Id, month.Value, year.Value));

        return true;
    }

    public Operation<bool, string> Dismiss(Position position)
    {
        if (_employeePositions is null || !_employeePositions.Any())
            return Operation.Error("EmployeePositions is null or empty!");

        var positionIndex = _employeePositions.FindIndex(x => x.Position.Id == position.Id);

        if(positionIndex < 0)
            return Operation.Error("Position not found!");

        _employeePositions.RemoveAt(positionIndex);

        RaiseDomainEvent(new DismissFromPositionDomainEvent(Id, position.Id));

        return true;
    }
}
