using Calabonga.OperationResults;
using Motivation.Domain.Entities.Base;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public sealed class PositionKpi : Auditable
{
    private PositionKpi(
        Guid id,
        int orderNumber,
        WeightValue weight,
        DateTime validFrom,
        DateTime validTo)
    {
        Id = id;
        OrderNumber = orderNumber;
        Weight = weight;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    public Guid Id { get; }
    public DateTime ValidFrom { get; private set; }
    public DateTime ValidTo { get; private set; }
    public TargetValue Target { get; private set; } = null!;
    public FactValue Fact { get; private set; } = null!;
    public AchievementValue Achievement { get; private set; } = null!;
    public BonusAmountValue BonusAmount { get; private set; } = null!;
    public int OrderNumber { get; private set; }
    public WeightValue Weight  { get; private set; } = null!;

    public Position Position { get; private set; } = null!;
    public Kpi Kpi { get; private set; } = null!;


    public static Operation<PositionKpi, string> Create(
        Kpi kpi,
        int orderNumber,
        WeightValue weight,
        DateTime validFrom,
        DateTime validTo = default) 
    {
        if (kpi is null)
            return Operation.Error("Kpi is null!");
        if (weight is null)
            return Operation.Error("Weight is null!");

        var positionKpi = new PositionKpi(Guid.CreateVersion7(), orderNumber, weight, validFrom, validTo);

        var kpiResult = positionKpi.SetKpi(kpi);
        if (!kpiResult.Ok)
            return Operation.Error(kpiResult.Error);

        return positionKpi;
    }
    private Operation<bool, string> SetKpi(Kpi kpi)
    {
        if (kpi is null)
            return Operation.Error("Kpi is null!");
        Kpi = kpi;
        return true;
    }

}
