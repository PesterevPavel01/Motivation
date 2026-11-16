using Calabonga.OperationResults;
using Motivation.Domain.DomainEvents.KpiDomainEvents;
using Motivation.Domain.Entities.Base;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities.Kpis;

public abstract class Kpi : AggregateRoot
{
    private readonly List<KpiFilter> _filters = [];

    protected Kpi(TitleValue title, CodeValue code, Guid id, AbbreviationValue abbreviation, CalculationType calculationType) : base(id, code)
    {
        Abbreviation = abbreviation;
        CalculationType = calculationType;
        Title = title;
    }

    public AbbreviationValue Abbreviation { get; protected set; } = null!;
    public CalculationType CalculationType { get; private set; }
    public MeasurementUnit MeasurementUnit { get; private set; } = null!;
    public TitleValue Title { get; protected set; }

    public IReadOnlyList<KpiFilter> Filters => _filters.AsReadOnly();

    public virtual KpiType KpiType { get; private set; }

    public Operation<bool, string> SetMeasurementUnit(MeasurementUnit measurementUnit)
    {
        if (measurementUnit is null)
            return Operation.Error("Measurement unit is null!");
        MeasurementUnit = measurementUnit;
        return true;
    }
    public Operation<bool, string> SetCalculationType(CalculationType calculationType)
    {
        CalculationType = calculationType;
        return true;
    }
    protected Operation<bool, string> SetFilters(List<KpiFilter> filters)
    {
        if(filters is null)
            return Operation.Error("KpiFilters is null!");
        var clearResult = ClearFilters();
        if(!clearResult.Ok)
            return Operation.Error(clearResult.Error);
        _filters.AddRange(filters);
        return true;
    }
    protected Operation<bool, string> ClearFilters()
    {
        _filters.Clear();
        return true;
    }

    public Operation<bool, string> Delete()
    {
        RaiseDomainEvent(new KpiDeletedDomainEvent(Id));

        return true;
    }
}
