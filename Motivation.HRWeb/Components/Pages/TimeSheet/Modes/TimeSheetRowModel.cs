namespace Motivation.HRWeb.Components.Pages.TimeSheet.Modes;

public class TimeSheetRowModel
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeFullName { get; set; } = string.Empty;
    public string PositionCode { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public int? Value { get; set; }
    public int EditingValue { get; set; }
    public bool IsSaving { get; set; } = false;
}