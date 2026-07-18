using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.ValueObjects;

public sealed class AchievementValue : ValueObject
{
    private const decimal MinValue = 0;

    private AchievementValue(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public static Operation<AchievementValue, string> Create(decimal value)
    {
        if (value < MinValue)
            return Operation.Error(
                $"Achievement cannot be less than {MinValue} (percent)");
        var roundedValue = Math.Round(value, 2);
        return new AchievementValue(roundedValue);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}