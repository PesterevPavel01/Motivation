using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;

namespace Motivation.AnalyticsCore.Domain.ValueObjects;

public class SupplierValue : ValueObject
{
    public const int MaxTitleLength = 256;

    private SupplierValue(string responsible)
    {
        Value = responsible;
    }

    public string Value { get; }

    public static Operation<SupplierValue, string> Create(string purchaser)
    {
        if (string.IsNullOrWhiteSpace(purchaser))
            return Operation.Error("Supplier is null or empty.");

        if (purchaser.Length > MaxTitleLength)
            return Operation.Error("Supplier length is greater than Max value.");

        return new SupplierValue(purchaser);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}