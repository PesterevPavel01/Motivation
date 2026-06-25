namespace Motivation.HRWeb.Components.Pages.Employees.Models;

public class EditEmployeeModel
{
    public string Code { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? SecondName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}
