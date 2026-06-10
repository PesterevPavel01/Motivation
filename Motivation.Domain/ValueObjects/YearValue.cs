using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;

namespace Motivation.Domain.ValueObjects;


public class YearValue : ValueObject
{
    public const int MinYear = 2000;
    public const int MaxYear = 2100;

    public YearValue(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public static Operation<YearValue, string> Create(int year)
    {
        if (year < MinYear || year > MaxYear)
            return Operation.Error($"Year must be between {MinYear} and {MaxYear}.");

        return new YearValue(year);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}