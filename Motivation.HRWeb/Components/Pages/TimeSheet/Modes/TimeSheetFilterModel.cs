namespace Motivation.HRWeb.Components.Pages.TimeSheet.Modes;

public class TimeSheetFilterModel
{
    public int Month { get; set; } = DateTime.Today.Month;
    public int Year { get; set; } = DateTime.Today.Year;
}
