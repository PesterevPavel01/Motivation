namespace Motivation.Contracts.Dto;

public sealed record SetDeductionDto(string? Code, string? Title, string PositionCode, decimal Value, int Month, int Year);
