using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.ValueObjects;

public sealed class FullNameValue : ValueObject
{
    public const int MaxFullNameLength = 256;

    private FullNameValue(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Operation<FullNameValue, string> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Operation.Error("Full name is null or empty.");

        if (value.Length > MaxFullNameLength)
            return Operation.Error("Full name length is greater than Max value.");

        return new FullNameValue(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
