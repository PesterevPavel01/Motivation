namespace Motivation.HRWeb.Components.Pages.HomePage.Models;

public class EditableFactCellModel
{
    public decimal Value { get; set; }
    public bool IsManual { get; set; }
    public Guid? TargetId { get; set; }
}
