using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.ValueObjects;

public class StandardHoursValue : ValueObject
{
    public const int MinValue = 0;
    public const int MaxValue = 255;

    public StandardHoursValue(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public static Operation<StandardHoursValue, string> Create(int value)
    {
        if (value < MinValue)
            return Operation.Error($"Standard hours cannot be less than {MinValue}.");

        if (value > MaxValue)
            return Operation.Error($"Standard hours cannot be greater than {MaxValue}.");

        return new StandardHoursValue(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
