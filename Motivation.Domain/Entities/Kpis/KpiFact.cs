using Calabonga.OperationResults;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities.Kpis;

public sealed class KpiFact
{
    private KpiFact(
        FactValue fact, 
        AchievementValue achievement, 
        BonusPercentValue bonusPercent, 
        BonusAmountValue bonusAmount) 
    {
        Fact = fact;
        Achievement = achievement;
        BonusPercent = bonusPercent;
        BonusAmount = bonusAmount;
    }

    public FactValue Fact { get; private set; } = null!;
    public AchievementValue Achievement { get; private set; } = null!;
    public BonusPercentValue BonusPercent { get; private set; } = null!;
    public BonusAmountValue BonusAmount { get; private set; } = null!;

    public static Operation<KpiFact, string> Create(
        FactValue fact,
        AchievementValue achievement,
        BonusPercentValue bonusPercent,
        BonusAmountValue bonusAmount)
    {
        if (fact is null)
            return Operation.Error("Fact is null!");
        if (achievement == default)
            return Operation.Error("Achievement is default!");
        if (bonusPercent == default)
            return Operation.Error("BonusPercent is default!");
        if (bonusAmount == default)
            return Operation.Error("BonusAmount is default!");

        return new KpiFact(fact, achievement, bonusPercent, bonusAmount);
    }

    public Operation<bool, string> Edit(
        FactValue fact,
        AchievementValue achievement,
        BonusPercentValue bonusPercent,
        BonusAmountValue bonusAmount)
    {
        Fact = fact;
        Achievement = achievement;
        BonusPercent = bonusPercent;
        BonusAmount = bonusAmount;
        return true;
    }
}
