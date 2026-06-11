using Calabonga.OperationResults;
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

    public KpiType Type { get; private set; }
    public AbbreviationValue Abbreviation { get; private set; } = null!;
    public CalculationType CalculationType { get; private set; }
    public MeasurementUnit MeasurementUnit { get; private set; } = null!;
    public TitleValue Title { get; protected set; }

    public IReadOnlyList<KpiFilter> Filters => _filters.AsReadOnly();

    public virtual KpiType KpiType { get; private set; }

    protected Operation<bool, string> SetMeasurementUnit(MeasurementUnit measurementUnit)
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
