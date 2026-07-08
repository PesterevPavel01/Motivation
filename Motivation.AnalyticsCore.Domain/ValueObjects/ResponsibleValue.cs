using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;

namespace Motivation.AnalyticsCore.Domain.ValueObjects;

public class ResponsibleValue : ValueObject
{
    public const int MaxTitleLength = 256;

    private ResponsibleValue(string responsible)
    {
        Value = responsible;
    }

    public string Value { get; }

    public static Operation<ResponsibleValue, string> Create(string purchaser)
    {
        if (string.IsNullOrWhiteSpace(purchaser))
            return Operation.Error("Responsible is null or empty.");

        if (purchaser.Length > MaxTitleLength)
            return Operation.Error("Responsible length is greater than Max value.");

        return new ResponsibleValue(purchaser);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}