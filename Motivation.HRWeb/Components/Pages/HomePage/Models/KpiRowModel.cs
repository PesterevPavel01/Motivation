using Motivation.Application.Mappings;
using Motivation.Domain.Enum;
using Motivation.HRWeb.Components.Pages.HomePage.ViewModels;

namespace Motivation.HRWeb.Components.Pages.HomePage.Models;

public class KpiRowModel
{
    public string Type { get; set; } = string.Empty;
    public string KpiTitle { get; set; } = string.Empty;
    public string MeasurementUnit { get; set; } = string.Empty;
    public Guid? TargetId { get; set; }
    public decimal Target { get; set; }
    public decimal MotivationPart { get; set; } 
    public decimal Fact { get; set; }
    public decimal Achievement { get; set; }
    public decimal BonusPercent { get; set; }
    public decimal Bonus { get; set; }
    public int OrderNumber { get; set; }

    public bool IsManual => Type == KpiType.Manual.ToRussianString();
}
