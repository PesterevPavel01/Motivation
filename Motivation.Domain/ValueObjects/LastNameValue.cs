using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.ValueObjects;

public sealed class LastNameValue : ValueObject
{
    public const int MaxLastNameLength = 50;

    private LastNameValue(string firstName)
    {
        Value = firstName;
    }

    public string Value { get; }

    public static Operation<LastNameValue, string> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Operation.Error("Last name is null or empty.");

        if (value.Length > MaxLastNameLength)
            return Operation.Error("Last name length is greater than Max value.");

        return new LastNameValue(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
