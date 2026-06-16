using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.ValueObjects;

public class HoursValue : ValueObject
{
    public const int MinValue = 0;
    public const int MaxValue = 255;

    public HoursValue(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public static Operation<HoursValue, string> Create(int value)
    {
        if (value < MinValue)
            return Operation.Error($"Hours cannot be less than {MinValue}.");

        if (value > MaxValue)
            return Operation.Error($"Hours cannot be greater than {MaxValue}.");

        return new HoursValue(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
