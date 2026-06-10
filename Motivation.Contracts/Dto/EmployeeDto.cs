namespace Motivation.Contracts.Dto;

public record EmployeeDto() 
{
    public required string Code { get; set; }
    public required string FirstName { get; set; }
    public required string? SecondName { get; set; }
    public required string LastName { get; set; }
    public required string FullName { get; set; }
    public IEnumerable<EmployeePositionDto> Positions { get; set; } = [];
}
