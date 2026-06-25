using Motivation.Contracts.Dto;
using Motivation.HRWeb.Components.Pages.Employees.Models;
using Motivation.HRWeb.Components.Pages.EmployeesPage.Models;

namespace Motivation.HRWeb.Application.Mappings;

public static class EmployeeMapper
{
    public static EmployeeCreateDto ToCreateDto(this CreateEmployeeModel model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        return new EmployeeCreateDto(
            Code: model.Code,
            FirstName: model.FirstName,
            SecondName: model.SecondName,
            LastName: model.LastName,
            FullName: model.FullName
        );
    }

    public static EmployeeUpdateDto ToUpdateDto(this EditEmployeeModel model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        return new EmployeeUpdateDto(
            Code: model.Code,
            FirstName: model.FirstName,
            SecondName: model.SecondName,
            LastName: model.LastName,
            FullName: model.FullName
        );
    }

    public static AssignEmployeeToPositionDto ToAssignDto(this AssignEmployeeModel model)
    {
        return new AssignEmployeeToPositionDto(
            PositionCode: model.SelectedPositionCode!,
            AssignmentDate: model.AssignmentDate
        );
    }
}
