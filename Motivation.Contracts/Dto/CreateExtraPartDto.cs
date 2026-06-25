namespace Motivation.Contracts.Dto;

public sealed record CreateExtraPartDto(string? Code, string? Title, string PositionCode, decimal Value, DateTime ValidFrom, DateTime ValidTo);
