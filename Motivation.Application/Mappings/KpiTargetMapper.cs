using Motivation.Contracts.Dto;
using Motivation.Domain.Entities.Kpis;

namespace Motivation.Application.Mappings;

public static class KpiTargetMapper
{
    public static KpiTargetDto? MapToDto(this KpiTarget? source)
    {
        if (source is null || source.Employee is null)
        {
            return null;
        }

        return new KpiTargetDto {
            Id = source.Id,
            EmployeeCode = source.Employee.Code.Value,
            Target = source.Target.Value,
            ValidFrom = source.ValidFrom,
            ValidTo = source.ValidTo,
            Fact = source.KpiFact?.MapToDto()
        };
    }
}
