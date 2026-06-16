using Calabonga.OperationResults;
using Motivation.Domain.Enum;

namespace Motivation.Application.Mappings;

public static class WorkWeekTypeMapper
{
    public static Operation<WorkWeekType, string> FromRussianString(string workWeekType)
    {
        return workWeekType.Trim().ToLower() switch
        {
            "пятидневка" => WorkWeekType.FiveDayWeek,
            "шестидневка" => WorkWeekType.SixDayWeek,
            _ => Operation.Error($"Unknown calculation type: {workWeekType}")
        };
    }
}
public static class WorkWeekTypeExtensions
{
    public static string ToRussianString(this WorkWeekType type)
    {
        return type switch
        {
            WorkWeekType.FiveDayWeek => "пятидневка",
            WorkWeekType.SixDayWeek => "шестидневка",
            _ => type.ToString()
        };
    }
}
