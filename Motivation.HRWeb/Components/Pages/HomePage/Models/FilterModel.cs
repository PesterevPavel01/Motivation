using Motivation.Contracts.Dto;

namespace Motivation.HRWeb.Components.Pages.HomePage.Models;

public class FilterModel
{
    public int Month { get; set; } = DateTime.Today.Month;
    public int Year { get; set; } = DateTime.Today.Year;
    public string? EmployeeCode { get; set; }
    public string? PositionCode { get; set; }
    public List<EmployeeDto>? Employees { get; set; }
    public List<PositionItem>? Positions { get; set; }
}