namespace Motivation.HRWeb.Components.Pages.Employees.Models;

public class CreateEmployeeModel
{
    public string? Code { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? SecondName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    public void Reset()
    {
        Code = null;
        FirstName = string.Empty;
        SecondName = null;
        LastName = string.Empty;
        FullName = string.Empty;
    }
}