public class PositionKpiItemModel
{
    public string? KpiCode { get; set; }
    public string? KpiTitle { get; set; }
    public int OrderNumber { get; set; }
    public decimal Weight { get; set; }
    public DateTime ValidFrom { get; set; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    public DateTime ValidTo { get; set; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1);
    public bool IsEditing { get; set; }
    public bool IsNew { get; set; }

    public bool IsValid => !string.IsNullOrEmpty(KpiCode) &&
                          OrderNumber > 0 &&
                          Weight > 0 &&
                          ValidFrom != default &&
                          ValidTo != default &&
                          ValidTo > ValidFrom;
}