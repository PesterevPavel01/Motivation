using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.ValueObjects;

public sealed class WeightValue : ValueObject
{
    private const decimal MinValue = 0;
    private const decimal MaxValue = 100;

    private WeightValue(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public static Operation<WeightValue, string> Create(decimal value)
    {
        if (value < MinValue || value > MaxValue)
            return Operation.Error(
                $"Weight must be between {MinValue} and {MaxValue} (percent)");
        var roundedValue = Math.Round(value, 2);
        return new WeightValue(roundedValue);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}