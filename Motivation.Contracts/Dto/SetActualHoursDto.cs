namespace Motivation.Contracts.Dto;

public sealed record SetActualHoursDto(string PositionCode, int Value, int Month, int Year);
