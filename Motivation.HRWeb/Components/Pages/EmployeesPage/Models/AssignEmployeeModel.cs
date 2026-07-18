namespace Motivation.HRWeb.Components.Pages.EmployeesPage.Models;

public class AssignEmployeeModel
{
    public string? SelectedPositionCode { get; set; }
    public DateTime AssignmentDate { get; set; } = DateTime.Today;

    public void Reset()
    {
        SelectedPositionCode = null;
        AssignmentDate = DateTime.Today;
    }
}
