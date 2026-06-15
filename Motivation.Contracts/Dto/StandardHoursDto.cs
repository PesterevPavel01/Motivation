namespace Motivation.Contracts.Dto;

public sealed record StandardHoursDto(Guid Id, int Value, int Month, int Year, string WorkWeekType);
public sealed record StandardHoursUpdateDto(int Value);
