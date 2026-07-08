using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;

namespace Motivation.AnalyticsCore.Domain.ValueObjects;

public sealed class QuantityValue : ValueObject
{
    private QuantityValue(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public static Operation<QuantityValue, string> Create(decimal salary)
    {
        return new QuantityValue(salary);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}