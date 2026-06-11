using Calabonga.OperationResults;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities.Kpis;

public sealed class SalesKpi : Kpi
{
    public SalesKpi(TitleValue title, CodeValue code, Guid id, AbbreviationValue abbreviation, CalculationType calculationType) 
        : base(title, code, id, abbreviation, calculationType){}

    public override KpiType KpiType => KpiType.Sales;
    public static Operation<SalesKpi, string> Create(TitleValue title, CodeValue code, AbbreviationValue abbreviation, CalculationType calculationType, MeasurementUnit measurementUnit)
    {
        if (title is null)
            return Operation.Error("Title is null!");
        if (code is null)
            return Operation.Error("Code is null!");
        if (abbreviation is null)
            return Operation.Error("Abbreviation is null!");

        var kpi = new SalesKpi(title, code, Guid.CreateVersion7(), abbreviation, calculationType);

        var setMeasurementUnit = kpi.SetMeasurementUnit(measurementUnit);
        if (!setMeasurementUnit.Ok)
            return Operation.Error(setMeasurementUnit.Error);

        return kpi;
    }
}
