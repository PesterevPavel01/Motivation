namespace Motivation.Contracts.Dto.CalculateDto;

public sealed class KpiCalculationTaskResult
{
    public DateTime PeriodFrom { get; set; }
    public DateTime PeriodTo { get; set; }
    public required Guid PositionKpiId { get; set; }
    public decimal Target { get; set; }
    public decimal Value { get; set; }
    public decimal Achievement { get; set; }
}
