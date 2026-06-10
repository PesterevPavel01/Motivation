using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;

namespace Motivation.Application.Mappings;

public static class EmployeePositionMapper
{
    public static EmployeePositionDto MapToDto(this EmployeePosition source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return new EmployeePositionDto
        {
            Position = source.Position.MapToDto(),
            AssignmentDate = source.AssignmentDate,
            ExtraPartHistory = source.ExtraPartHistory?.Select(x => x.MapToDto()),
            DeductionHistory = source.DeductionHistory?.Select(x => x.MapToDto())
        };
    }
}
