using Motivation.Contracts.Dto;

namespace Motivation.HRWeb.Components.Pages.TargetPage.Models;

public class TargetsFilterModel
{
    public int Month { get; set; } = DateTime.Today.Month;
    public int Year { get; set; } = DateTime.Today.Year;
    public string? EmployeeCode { get; set; }
    public List<EmployeeDto>? Employees { get; set; } = [];
}
