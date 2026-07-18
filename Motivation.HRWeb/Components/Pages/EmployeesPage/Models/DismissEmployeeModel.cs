namespace Motivation.HRWeb.Components.Pages.EmployeesPage.Models;

public class DismissEmployeeModel
{
    public string? SelectedPositionCode { get; set; }
    public List<EmployeePositionItem> AvailablePositions { get; private set; } = [];

    public void Reset()
    {
        SelectedPositionCode = null;
        AvailablePositions = [];
    }

    public void SetAvailablePositions(List<EmployeePositionItem> positions)
    {
        AvailablePositions = positions;
    }
}