namespace Motivation.Contracts.Dto;

public sealed record PositionDto(string Title, string Code, decimal BaseSalary, decimal MotivationPart, bool RecalculateToHours, string WorkWeekType);
