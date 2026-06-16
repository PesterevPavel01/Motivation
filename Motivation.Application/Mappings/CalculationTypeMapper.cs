using Calabonga.OperationResults;
using Motivation.Domain.Enum;

namespace Motivation.Application.Mappings;

public static class CalculationTypeMapper
{
    public static Operation<CalculationType, string> FromRussianString(string calculationType)
    {
        return calculationType.Trim().ToLower() switch
        {
            "стандарт" => CalculationType.Standard,
            "обратный" => CalculationType.Reversed,
            "сдельный" => CalculationType.Progressive,
            _ => Operation.Error($"Unknown calculation type: {calculationType}")
        };
    }
}

public static class CalculationTypeExtensions
{
    public static string ToRussianString(this CalculationType type)
    {
        return type switch
        {
            CalculationType.Standard => "стандарт",
            CalculationType.Reversed => "обратный",
            CalculationType.Progressive => "сдельный",
            _ => type.ToString()
        };
    }
}