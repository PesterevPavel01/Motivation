namespace Motivation.Contracts.Dto;

public sealed class KpiTargetDto
{
        public decimal Target {  get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public KpiFactDto? Fact { get; set; }
}
