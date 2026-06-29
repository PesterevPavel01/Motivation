namespace Motivation.Contracts.Dto;

public sealed record KpiUpdateDto()
{
    public required string KpiType { get; set; }
    public required string Title { get; set; }
    public required string Code { get; set; }
    public required string Abbreviation { get; set; }
    public required string CalculationType { get; set; }
    public required string MeasurementUnitCode { get; set; }
    public IEnumerable<KpiFilterDto> CustomerFilter { get; set; } = [];
    public IEnumerable<KpiFilterDto> ProductFilter { get; set; } = [];
    public IEnumerable<KpiFilterDto> CompanyFilter { get; set; } = [];
    public IEnumerable<KpiFilterDto> CustomerResponsibleFilter { get; set; } = [];
    public IEnumerable<KpiFilterDto> SalesResponsibleFilter { get; set; } = [];
    public IEnumerable<KpiFilterDto> ProductResponsibleFilter { get; set; } = [];
}