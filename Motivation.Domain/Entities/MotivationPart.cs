using Calabonga.OperationResults;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public class MotivationPart
{
    private MotivationPart(MotivationPartValue motivationPartValue)
    {
        MotivationPartValue = motivationPartValue;
    }

    public MotivationPartValue MotivationPartValue { get; private set; }
    public bool RecalculateToHours { get; private set; } = true;

    public static Operation<MotivationPart, string> Create(MotivationPartValue motivationPartValue) 
    {
        if (motivationPartValue is null)
            return Operation.Error("Motivation part is null!");

        return new MotivationPart(motivationPartValue);
    }

    public Operation<bool, string> SetRecalculateToHours(bool recalculateToHours) 
    {
        RecalculateToHours = recalculateToHours;
        return true;
    }
}
