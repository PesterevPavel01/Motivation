namespace Motivation.Contracts.Dto;

public sealed record EmployeePositionDto() 
{
    public PositionDto? Position {  get; set; }
    public DateTime AssignmentDate { get; set; }
    public IEnumerable<ExtraPartDto?>? ExtraPartHistory { get; set; } = [];
    public IEnumerable<DeductionDto?>? DeductionHistory { get; set; } = [];
};
