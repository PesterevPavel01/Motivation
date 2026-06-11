namespace Motivation.Contracts.Dto
{
    public sealed record KpiDto()
    {
        public required string Title { get; set; }
        public required string Code { get; set; }
        public required string Abbreviation { get; set; }
        public required string CalculationType { get; set; }
        public required string MeasurementUnit { get; set; }
        public required string KpiType { get; set; }
        public IEnumerable<string> CustomerFilter { get; set; } = [];
        public IEnumerable<string> ProductFilter { get; set; } = [];
        public IEnumerable<string> CompanyFilter { get; set; } = [];
    }
}
