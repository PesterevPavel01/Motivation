namespace Motivation.Contracts.Dto;

public sealed record PositionUpdateDto(string Title, string? Code, decimal BaseSalary, decimal MotivationPart, bool RecalculateToHours, string WorkWeekType);
