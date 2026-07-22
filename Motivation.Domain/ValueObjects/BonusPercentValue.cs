using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.ValueObjects;

public sealed class BonusPercentValue : ValueObject
{
    private BonusPercentValue(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public static Operation<BonusPercentValue, string> Create(decimal value)
    {
        if (value < 0)
            return Operation.Error("Bonus percent must be less than or equal to zero!");
        return new BonusPercentValue(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}