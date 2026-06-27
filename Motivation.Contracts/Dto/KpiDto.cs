namespace Motivation.Contracts.Dto
{
    public sealed record KpiDto()
    {
        public required string Title { get; set; }
        public required string Code { get; set; }
        public required string Abbreviation { get; set; }
        public required string CalculationType { get; set; }
        public required MeasurementUnitDto MeasurementUnit { get; set; }
        public required string KpiType { get; set; }
        public IEnumerable<KpiFilterDto> CustomerFilter { get; set; } = [];
        public IEnumerable<KpiFilterDto> ProductFilter { get; set; } = [];
        public IEnumerable<KpiFilterDto> CompanyFilter { get; set; } = [];
    }
}
