namespace Motivation.HRWeb.Components.Pages.HomePage.Models;

public class EditableFactCellSaveResult
{
    public KpiRowModel Row { get; set; } = null!;
    public decimal NewValue { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
