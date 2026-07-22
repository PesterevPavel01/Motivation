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
    public IEnumerable<KpiFilterDto> CustomerFilter { get; set; } = [];
    public IEnumerable<KpiFilterDto> SupplierFilter { get; set; } = [];
    public IEnumerable<KpiFilterDto> ProductFilter { get; set; } = [];
    public IEnumerable<KpiFilterDto> ProductStatusFilter { get; set; } = [];
    public IEnumerable<KpiFilterDto> CompanyFilter { get; set; } = [];
    public IEnumerable<KpiFilterDto> CustomerResponsibleFilter { get; set; } = [];
    public IEnumerable<KpiFilterDto> SalesResponsibleFilter { get; set; } = [];
    public IEnumerable<KpiFilterDto> ProductResponsibleFilter { get; set; } = [];
}
