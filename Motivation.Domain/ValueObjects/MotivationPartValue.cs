using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.ValueObjects;

public class MotivationPartValue : ValueObject
{
    public const int MinValue = 0;

    public MotivationPartValue(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public static Operation<MotivationPartValue, string> Create(decimal value)
    {
        if (value < MinValue)
            return Operation.Error($"Motivation part cannot be less than {MinValue}.");

        return new MotivationPartValue(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
