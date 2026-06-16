using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities
{
    public sealed class ActualHours : Auditable
    {
        private ActualHours(HoursValue hours, MonthValue month, YearValue year) 
        {
            Month = month;
            Year = year;
            Hours = hours;
        }

        public MonthValue Month { get; private set; }
        public YearValue Year { get; private set; }
        public HoursValue Hours { get; private set; }

        public static Operation<ActualHours, string> Create(HoursValue hours, MonthValue month, YearValue year)
        {
            if (hours is null)
                return Operation.Error("Hours is null!");
            if (month is null)
                return Operation.Error("Month is null!");
            if (year is null)
                return Operation.Error("Year is null!");

            return new ActualHours(hours, month, year);
        }
    }
}
