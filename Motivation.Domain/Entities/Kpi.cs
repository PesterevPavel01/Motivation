using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public class Kpi : SimpleEntity
{
    private readonly List<KpiFilter> _filters = [];

    private Kpi(TitleValue title, CodeValue code, Guid id, AbbreviationValue abbreviation, CalculationType calculationType) : base(title, code, id)
    {
        Abbreviation = abbreviation;
        CalculationType = calculationType;
    }

    public KpiType Type { get; private set; }
    public AbbreviationValue Abbreviation { get; private set; } = null!;
    public CalculationType CalculationType { get; private set; }
    public MeasurementUnit MeasurementUnit { get; private set; } = null!;

    public IReadOnlyList<KpiFilter> Filters => _filters.AsReadOnly();

    public static Operation<Kpi, string> Create(TitleValue title, CodeValue code, AbbreviationValue abbreviation, CalculationType calculationType, MeasurementUnit measurementUnit) 
    {
        if (title is null)
            return Operation.Error("Title is null!");
        if (code is null)
            return Operation.Error("Code is null!");
        if (abbreviation is null)
            return Operation.Error("Abbreviation is null!");

        var kpi = new Kpi(title, code, Guid.CreateVersion7(), abbreviation, calculationType);
        
        var setMeasurementUnit = kpi.SetMeasurementUnit(measurementUnit);
        if (!setMeasurementUnit.Ok)
            return Operation.Error(setMeasurementUnit.Error);
        
        return kpi;
    }
    private Operation<bool, string> SetMeasurementUnit(MeasurementUnit measurementUnit)
    {
        if (measurementUnit is null)
            return Operation.Error("Measurement unit is null!");
        MeasurementUnit = measurementUnit;
        return true;
    }
    public void AddFilter(KpiFilter item)
    {
        _filters.Add(item);
    }
}
