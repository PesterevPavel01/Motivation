namespace Motivation.Contracts.Dto;

public sealed class PositionKpiDto()
{
    public required string KpiCode { get; set; }
    public string? KpiTitle { get; set; }
    public int OrderNumber { get; set; }
    public decimal Weight { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public List<KpiTargetDto> Targets { get; set; } = [];
    public KpiDto? Kpi { get; set; }
}
