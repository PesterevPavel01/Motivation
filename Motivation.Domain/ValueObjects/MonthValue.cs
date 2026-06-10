using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.ValueObjects;

public class MonthValue : ValueObject
{
    public const int MinMonth = 1;
    public const int MaxMonth = 12;

    public MonthValue(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public static Operation<MonthValue, string> Create(int month)
    {
        if (month < MinMonth || month > MaxMonth)
            return Operation.Error($"Month must be between {MinMonth} and {MaxMonth}.");

        return new MonthValue(month);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}