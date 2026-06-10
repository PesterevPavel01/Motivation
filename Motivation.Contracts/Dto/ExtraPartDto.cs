namespace Motivation.Contracts.Dto;

public sealed record ExtraPartDto(string Code, string Title, string EmployeeCode, string PositionCode, decimal Value, DateTime ValidFrom, DateTime ValidTo);