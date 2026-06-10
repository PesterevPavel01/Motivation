using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.ValueObjects;

public sealed class BonusAmountValue : ValueObject
{
    private BonusAmountValue(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public static Operation<BonusAmountValue, string> Create(decimal value)
    {
        if (value < 0)
            return Operation.Error("Bonus amount must be less than or equal to zero!");
        return new BonusAmountValue(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}