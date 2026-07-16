namespace Motivation.HRWeb.Components.Pages.TargetPage.Models;

public class KpiTargetItemModel
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeTitle { get; set; } = string.Empty;
    public string PositionCode { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string KpiCode { get; set; } = string.Empty;
    public string KpiTitle { get; set; } = string.Empty;
    public decimal? Target { get; set; }
    public int OrderNumber { get; set; }
    public bool IsEditing { get; set; }
    public decimal? EditValue { get; set; }
    public bool IsSaving { get; set; }
}
