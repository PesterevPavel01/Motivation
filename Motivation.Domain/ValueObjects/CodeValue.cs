using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.ValueObjects;

public sealed class CodeValue : ValueObject
{
    public const int MaxCodeLength = 50;

    private CodeValue(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Operation<CodeValue, string> Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Operation.Error("Value is null or empty.");

        if (title.Length > MaxCodeLength)
            return Operation.Error("Value length is greater than Max value.");

        return new CodeValue(title);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}