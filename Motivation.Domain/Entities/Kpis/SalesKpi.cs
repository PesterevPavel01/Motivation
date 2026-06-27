using Calabonga.OperationResults;
using Motivation.Domain.Enum;
using Motivation.Domain.Events.KpiDomainEvents;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities.Kpis;

public sealed class SalesKpi : Kpi
{
    public SalesKpi(TitleValue title, CodeValue code, Guid id, AbbreviationValue abbreviation, CalculationType calculationType) 
        : base(title, code, id, abbreviation, calculationType){}

    public override KpiType KpiType => KpiType.Sales;
    public static Operation<SalesKpi, string> Create(
        TitleValue title, 
        CodeValue code, 
        AbbreviationValue abbreviation, 
        CalculationType calculationType, 
        MeasurementUnit measurementUnit,
        List<KpiFilter> filters)
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

        var setFilters = kpi.SetFilters(filters);
        if (!setFilters.Ok)
            return Operation.Error(setFilters.Error);

        kpi.RaiseDomainEvent(new KpiCreatedDomainEvent(kpi.Id));

        return kpi;
    }

    public object Include(Func<object, object> value)
    {
        throw new NotImplementedException();
    }

    public Operation<SalesKpi, string> Update(
        TitleValue title,
        CodeValue code,
        CalculationType calculationType,
        AbbreviationValue abbreviation,
        MeasurementUnit measurementUnit,
        List<KpiFilter> filters) 
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

        var setCalculationType = SetCalculationType(calculationType);
        if (!setCalculationType.Ok)
            return Operation.Error(setCalculationType.Error);
        var setMeasurementUnit = SetMeasurementUnit(measurementUnit);
        if (!setMeasurementUnit.Ok)
            return Operation.Error(setMeasurementUnit.Error);

        var setFilters = SetFilters(filters);
        if (!setFilters.Ok)
            return Operation.Error(setFilters.Error);

        RaiseDomainEvent(new KpiUpdatedDomainEvent(Id));

        return this;
    }
}
