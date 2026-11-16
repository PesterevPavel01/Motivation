using Motivation.Contracts.Models;

namespace Motivation.Contracts.Dto.CalculateDto;

public sealed class KpiCalculationTask
{
    public DateTime PeriodFrom { get; set; }
    public DateTime PeriodTo { get; set; }
    public required Guid PositionKpiId { get; set; }
    public required Guid EmployeeId { get; set; }
    public required string CalculationType { get; set; }
    public required MeasurementUnitDto MeasurementUnit { get; set; }
    public required string KpiType { get; set; }
    public decimal Weight { get; set; }
    public decimal MotivationPart { get; set; }
    public decimal Target { get; set; }
    public IEnumerable<KpiFilterDto> KpiFilters { get; set; } = [];
    public ManualResultDto? ManualResult { get; set; }
}
