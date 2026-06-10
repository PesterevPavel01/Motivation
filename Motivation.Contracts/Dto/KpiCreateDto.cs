namespace Motivation.Contracts.Dto;

public sealed record KpiCreateDto()
{
    public required string Title { get; set; }
    public required string Code { get; set; }
    public required string Abbreviation { get; set; }
    public required string CalculationType {  get; set; }
    public required string MeasurementUnitCode { get; set; }
    public IEnumerable<string> CustomerFilter { get; set; } = [];
    public IEnumerable<string> ProductFilter { get; set; } = [];
    public IEnumerable<string> CompanyFilter { get; set; } = [];
}
