using Calabonga.OperationResults;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities.Kpis;

public sealed class KpiTarget
{
    public KpiTarget(Guid id, TargetValue target, DateTime validFrom, DateTime validTo)
    {
        Id = id;
        Target = target;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
    public Guid Id { get; }
    public DateTime ValidFrom { get; private set; }
    public DateTime ValidTo { get; private set; }
    public TargetValue Target { get; private set; } = null!;

    public KpiFact? KpiFact { get; private set; }
    public PositionKpi PositionKpi { get; private set; } = null!;

    public static Operation<KpiTarget, string> Create(
        TargetValue target,
        DateTime validFrom,
        DateTime validTo)
    {
        if (target is null)
            return Operation.Error("Target is null!");
        if(validFrom == default)
            return Operation.Error("ValidFrom is default!");
        if (validTo == default)
            return Operation.Error("ValidTo is default!");

        return new KpiTarget(Guid.CreateVersion7(), target, validFrom, validTo);
    }

    public Operation<bool, string> SetFact(KpiFact fact)
    {
        if (fact is null)
            return Operation.Error("KpiFact is null!");
        KpiFact = fact;
        return true;
    }
}
