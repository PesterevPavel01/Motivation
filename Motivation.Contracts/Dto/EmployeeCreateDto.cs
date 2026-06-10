namespace Motivation.Contracts.Dto;

public record EmployeeCreateDto(string Code, string FirstName, string? SecondName, string LastName, string FullName);
