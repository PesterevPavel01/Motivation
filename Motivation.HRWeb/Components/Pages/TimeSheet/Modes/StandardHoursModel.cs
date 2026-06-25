namespace Motivation.HRWeb.Components.Pages.TimeSheet.Modes;

public class StandardHoursModel
{
    public string WorkWeekType { get; set; } = string.Empty;
    public Guid? Id { get; set; }
    public int Value { get; set; }
    public bool IsEditing { get; set; }
    public bool IsSaving { get; set; }
}
