using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public class Employee : Entity
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
        return employee;
    }

    public Operation<bool, string> AssignToPosition(Position position, DateTime assignmentDate)
    {
        if (position is null)
            return Operation.Error("Position is null!");
        if (assignmentDate == default)
            return Operation.Error("AssignmentDate is default DateTime!");

        var employeePositionResult = EmployeePosition.Create(position, this, assignmentDate);
        if (!employeePositionResult.Ok)
            return Operation.Error(employeePositionResult.Error);

        _employeePositions.Add(employeePositionResult.Result);

        return true;
    }
}
