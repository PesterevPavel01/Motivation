using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public class StandardHours : Auditable
{
    private StandardHours(Guid id, StandardHoursValue standardHoursValue, WorkWeekType workWeekType, MonthValue month, YearValue year)
    {
        Id = id;
        WorkWeekType = workWeekType;
        Month = month;
        Year = year;
        StandardHoursValue = standardHoursValue;
    }

    public WorkWeekType WorkWeekType { get; private set; }
    public Guid Id { get; }
    public MonthValue Month { get; private set; }
    public YearValue Year { get; private set; }
    public StandardHoursValue StandardHoursValue { get; private set; }

    public static Operation<StandardHours, string> Create(StandardHoursValue standardHours, WorkWeekType weekType, MonthValue month, YearValue year) 
    {
        if (standardHours is null)
            return Operation.Error("Standard hours is null!");
        if (month is null)
            return Operation.Error("Month is null!");
        if (year is null)
            return Operation.Error("Year is null!");

        return new StandardHours(Guid.CreateVersion7(), standardHours, weekType, month, year);
    }

}
