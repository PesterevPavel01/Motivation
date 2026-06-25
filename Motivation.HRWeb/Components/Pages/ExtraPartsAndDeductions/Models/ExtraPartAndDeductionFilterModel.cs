namespace Motivation.HRWeb.Components.Pages.ExtraPartsAndDeductions.Models;

public class ExtraPartAndDeductionFilterModel
{
    public int Month { get; set; } = DateTime.Today.Month;
    public int Year { get; set; } = DateTime.Today.Year;
}