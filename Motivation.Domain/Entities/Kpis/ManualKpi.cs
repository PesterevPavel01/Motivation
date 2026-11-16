using Calabonga.OperationResults;
using Motivation.Domain.Enum;
using Motivation.Domain.Events.KpiDomainEvents;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities.Kpis;

public sealed class ManualKpi : Kpi
{
    public ManualKpi(TitleValue title, CodeValue code, Guid id, AbbreviationValue abbreviation, CalculationType calculationType)
        : base(title, code, id, abbreviation, calculationType) { }
    public override KpiType KpiType => KpiType.Manual;

    public static Operation<ManualKpi, string> Create(
        TitleValue title,
        CodeValue code,
        AbbreviationValue abbreviation,
        CalculationType calculationType,
        MeasurementUnit measurementUnit)
    {
        if (title is null)
            return Operation.Error("Title is null!");
        if (code is null)
            return Operation.Error("Code is null!");
        if (abbreviation is null)
            return Operation.Error("Abbreviation is null!");

        var kpi = new ManualKpi(title, code, Guid.CreateVersion7(), abbreviation, calculationType);

        var setMeasurementUnit = kpi.SetMeasurementUnit(measurementUnit);
        if (!setMeasurementUnit.Ok)
            return Operation.Error(setMeasurementUnit.Error);

        return kpi;
    }

    public Operation<ManualKpi, string> Update(
            TitleValue title,
            CodeValue code,
            AbbreviationValue abbreviation,
            MeasurementUnit measurementUnit)
    {
        if (title is null)
            return Operation.Error("Title is null!");
        if (code is null)
            return Operation.Error("Code is null!");
        if (abbreviation is null)
            return Operation.Error("Abbreviation is null!");

        Title = title;
        Code = code;
        Abbreviation = abbreviation;

        var setMeasurementUnit = SetMeasurementUnit(measurementUnit);
        if (!setMeasurementUnit.Ok)
            return Operation.Error(setMeasurementUnit.Error);

        RaiseDomainEvent(new KpiUpdatedDomainEvent(Id));

        return this;
    }
}