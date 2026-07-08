using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;

namespace Motivation.AnalyticsCore.Domain.ValueObjects;

public class ManagerValue : ValueObject
{
    public const int MaxManagerLength = 256;

    private ManagerValue(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Operation<ManagerValue, string> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Operation.Error("Manager is null or empty.");

        if (value.Length > MaxManagerLength)
            return Operation.Error("Manager length is greater than Max value.");

        return new ManagerValue(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
