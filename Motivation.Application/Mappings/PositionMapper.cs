using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.Enum;

namespace Motivation.Application.Mappings;

public static class PositionMapper
{
    public static PositionDto MapToDto(this Position source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return new PositionDto(
            Title: source.Title.Value,
            Code: source.Code.Value,
            BaseSalary: source.BaseSalary.Value,
            MotivationPart: source.MotivationPart.MotivationPartValue.Value,
            RecalculateToHours: source.MotivationPart.RecalculateToHours,
            WorkWeekType: source.WorkWeekType.ToRussianString()
        );
    }
}
