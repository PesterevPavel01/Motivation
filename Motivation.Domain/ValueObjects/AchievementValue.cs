using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.ValueObjects;

public sealed class AchievementValue : ValueObject
{
    private const decimal MinValue = 0;
    private const decimal MaxValue = 100;

    private AchievementValue(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public static Operation<AchievementValue, string> Create(decimal value)
    {
        if (value < MinValue || value > MaxValue)
            return Operation.Error(
                $"Achievement must be between {MinValue} and {MaxValue} (percent)");
        var roundedValue = Math.Round(value, 2);
        return new AchievementValue(roundedValue);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}