using Calabonga.OperationResults;
using Motivation.Domain.Enum;

namespace Motivation.Application.Mappings;

public static class CalculationTypeMapper
{
    public static Operation<CalculationType, string> FromRussianString(string calculationType)
    {
        return calculationType switch
        {
            "Стандарт" => CalculationType.Standard,
            "Обратный" => CalculationType.Reversed,
            "Сдельный" => CalculationType.Progressive,
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
            CalculationType.Standard => "Стандарт",
            CalculationType.Reversed => "Обратный",
            CalculationType.Progressive => "Сдельный",
            _ => type.ToString()
        };
    }
}