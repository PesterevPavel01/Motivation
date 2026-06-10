using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.ValueObjects;

public sealed class SecondNameValue : ValueObject
{
    public const int MaxSecondNameLength = 50;

    private SecondNameValue(string firstName)
    {
        Value = firstName;
    }

    public string Value { get; }

    public static Operation<SecondNameValue, string> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Operation.Error("Second name is null or empty.");

        if (value.Length > MaxSecondNameLength)
            return Operation.Error("Second name length is greater than Max value.");

        return new SecondNameValue(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
