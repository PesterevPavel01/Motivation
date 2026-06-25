namespace Motivation.HRWeb.Components.Pages.ExtraPartsAndDeductions.Models;

public class ExtraPartFormModel
{
    public string Title { get; set; } = "";
    public decimal Value { get; set; } = 0;
    public DateTime ValidFrom { get; set; } = DateTime.Today;
    public DateTime ValidTo { get; set; } = DateTime.Today.AddDays(30);

    public bool IsSaving { get; set; } = false;
    public string? ErrorMessage { get; set; }
}
