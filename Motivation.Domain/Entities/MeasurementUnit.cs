using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public class MeasurementUnit : SimpleEntity
{
    private readonly List<Kpi> _kpis = [];
    private MeasurementUnit(TitleValue title, CodeValue code, Guid id) : base(title, code, id)
    {
    }

    public IReadOnlyCollection<Kpi> Kpis => _kpis.AsReadOnly();

    public static Operation<MeasurementUnit, string> Create(TitleValue title, CodeValue code)
    {
        if (title is null)
            return Operation.Error("Title is null!");
        if (code is null)
            return Operation.Error("Code is null!");
        return new MeasurementUnit(title, code, Guid.CreateVersion7());
    }
}