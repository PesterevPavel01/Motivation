using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;

namespace Motivation.Application.Mappings;

public static class EmployeeMapper
{
    public static EmployeeDto MapToDto(this Employee source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return new EmployeeDto {
            Code = source.Code.Value,
            FirstName = source.FirstName.Value,
            SecondName = source.SecondName?.Value,
            LastName = source.LastName.Value,
            FullName = source.FullName.Value,
            Positions = source.EmployeePositions.Select(x => x.MapToDto())
        };
    }
}
