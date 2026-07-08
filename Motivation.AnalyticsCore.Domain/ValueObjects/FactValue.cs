using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;

namespace Motivation.AnalyticsCore.Domain.ValueObjects;

public sealed class FactValue : ValueObject
{
    private FactValue(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public static Operation<FactValue, string> Create(decimal value)
    {
        return new FactValue(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}