namespace Motivation.HRWeb.Components.Pages.PositionsPage.Models;

public class EditPositionModel
{
    public string Title { get; set; } = string.Empty;
    public string? Code { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal MotivationPart { get; set; }
    public bool RecalculateToHours { get; set; }
    public string WorkWeekType { get; set; } = string.Empty;
}
