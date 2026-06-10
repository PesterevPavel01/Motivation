using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.ValueObjects;

public sealed class ExtraPartValue : ValueObject
{
    private ExtraPartValue(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public static Operation<ExtraPartValue, string> Create(decimal value)
    {
        if (value <= 0)
            return Operation.Error("Extra part must be greater than zero!");

        return new ExtraPartValue(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}