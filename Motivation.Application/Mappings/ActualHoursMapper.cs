using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;

namespace Motivation.Application.Mappings;

public static class ActualHoursMapper
{
    public static ActualHoursDto MapToDto(this ActualHours? source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return new ActualHoursDto(
            Value: source.Hours.Value,
            Month: source.Month.Value,
            Year: source.Year.Value
        );
    }
}
