using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;

namespace Motivation.AnalyticsCore.Domain.ValueObjects;

public sealed class SalaryValue : ValueObject
{
    private SalaryValue(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public static Operation<SalaryValue, string> Create(decimal salary)
    {
        if (salary <= 0)
            return Operation.Error("Salary must be greater than zero!");

        return new SalaryValue(salary);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}