using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;

namespace Motivation.AnalyticsCore.Domain.ValueObjects;

public class PurchaserValue : ValueObject
{
    public const int MaxTitleLength = 256;

    private PurchaserValue(string purchaser)
    {
        Value = purchaser;
    }

    public string Value { get; }

    public static Operation<PurchaserValue, string> Create(string purchaser)
    {
        if (string.IsNullOrWhiteSpace(purchaser))
            return Operation.Error("Purchaser is null or empty.");

        if (purchaser.Length > MaxTitleLength)
            return Operation.Error("Purchaser length is greater than Max value.");

        return new PurchaserValue(purchaser);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}