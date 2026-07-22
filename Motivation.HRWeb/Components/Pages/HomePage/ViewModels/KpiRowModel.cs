namespace Motivation.HRWeb.Components.Pages.HomePage.ViewModels;

public class KpiRowModel
{
    public string KpiTitle { get; set; } = string.Empty;
    public string MeasurementUnit { get; set; } = string.Empty;
    public decimal Target { get; set; }
    public decimal MotivationPart { get; set; } 
    public decimal Fact { get; set; }
    public decimal Achievement { get; set; }
    public decimal BonusPercent { get; set; }
    public decimal Bonus { get; set; }
    public int OrderNumber { get; set; }
}
