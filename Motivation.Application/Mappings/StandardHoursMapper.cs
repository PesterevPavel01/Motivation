using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;

namespace Motivation.Application.Mappings;

public static class StandardHoursMapper
{
    public static StandardHoursDto MapToDto(this StandardHours? source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return new StandardHoursDto(
            Id: source.Id,
            Value: source.StandardHoursValue.Value,
            Month: source.Month.Value,
            Year: source.Year.Value,
            WorkWeekType: source.WorkWeekType.ToRussianString()
        );
    }
}

