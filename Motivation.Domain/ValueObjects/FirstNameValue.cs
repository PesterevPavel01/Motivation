using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.ValueObjects;

public class FirstNameValue : ValueObject
{
    public const int MaxFirstNameLength = 50;

    private FirstNameValue(string firstName)
    {
        Value = firstName;
    }

    public string Value { get; }

    public static Operation<FirstNameValue, string> Create(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Operation.Error("First name is null or empty.");

        if (firstName.Length > MaxFirstNameLength)
            return Operation.Error("First name length is greater than Max value.");

        return new FirstNameValue(firstName);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}