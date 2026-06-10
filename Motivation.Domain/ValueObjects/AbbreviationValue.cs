using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.ValueObjects;

public class AbbreviationValue : ValueObject
{
    public const int MaxLength = 50;

    private AbbreviationValue(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Operation<AbbreviationValue, string> Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Operation.Error("Value is null or empty.");

        if (title.Length > MaxLength)
            return Operation.Error("Value length is greater than Max value.");

        return new AbbreviationValue(title);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}