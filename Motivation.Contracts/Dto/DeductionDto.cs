namespace Motivation.Contracts.Dto;

public sealed record DeductionDto(string Code, string Title, string PositionCode, decimal Value, int Month, int Year);
