using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;

namespace Motivation.AnalyticsCore.Domain.ValueObjects;

public sealed class AmountValue : ValueObject
{
    private AmountValue(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public static Operation<AmountValue, string> Create(decimal value)
    {
        return new AmountValue(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}