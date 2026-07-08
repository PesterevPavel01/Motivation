using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;

namespace Motivation.AnalyticsCore.Domain.ValueObjects;

public sealed class TargetValue : ValueObject
{
    private TargetValue(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public static Operation<TargetValue, string> Create(decimal value)
    {
        if (value <= 0)
            return Operation.Error("Target value must be greater than zero!");

        return new TargetValue(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}