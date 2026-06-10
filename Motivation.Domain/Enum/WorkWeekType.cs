namespace Motivation.Domain.Enum;

public enum WorkWeekType
{
    SixDayWeek,
    FiveDayWeek
}

public static class WorkWeekTypeExtensions
{
    public static string ToRussianString(this WorkWeekType type)
    {
        return type switch
        {
            WorkWeekType.SixDayWeek => "шестидневка",
            WorkWeekType.FiveDayWeek => "пятидневка",
            _ => type.ToString()
        };
    }
}
