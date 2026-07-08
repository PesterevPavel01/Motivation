using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;

namespace Motivation.AnalyticsCore.Domain.ValueObjects;

public class StatusValue : ValueObject
{
    public const int MaxTitleLength = 256;

    private StatusValue(string responsible)
    {
        Value = responsible;
    }

    public string Value { get; }

    public static Operation<StatusValue, string> Create(string purchaser)
    {
        if (string.IsNullOrWhiteSpace(purchaser))
            return Operation.Error("Status is null or empty.");

        if (purchaser.Length > MaxTitleLength)
            return Operation.Error("Status length is greater than Max value.");

        return new StatusValue(purchaser);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}