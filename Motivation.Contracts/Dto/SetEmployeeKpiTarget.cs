namespace Motivation.Contracts.Dto;

public sealed class SetEmployeeKpiTarget
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string KpiCode { get; set; } = string.Empty;
    public decimal TargetValue { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}