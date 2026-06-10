using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public sealed class Deduction : SimpleEntity
{
    private Deduction(Guid id, TitleValue title, CodeValue code, DeductionValue deductionValue, MonthValue month, YearValue year) : base(title, code, id)
    {
        Month = month;
        Year = year;
        DeductionValue = deductionValue;
    }
    public MonthValue Month { get; private set; }
    public YearValue Year { get; private set; }
    public DeductionValue DeductionValue { get; private set; }

    public EmployeePosition EmployeePosition { get; private set; } = null!;

    public static Operation<Deduction, string> Create(TitleValue title, CodeValue code, DeductionValue deduction, MonthValue month, YearValue year)
    {
        if (title is null)
            return Operation.Error("Title is null!");
        if (code is null)
            return Operation.Error("Code is null!");
        if (deduction is null)
            return Operation.Error("Deduction is null!");
        if (month is null)
            return Operation.Error("Month is null!");
        if (year is null)
            return Operation.Error("Year is null!");

        return new Deduction(Guid.CreateVersion7(), title, code, deduction, month, year);
    }
}