using Motivation.Contracts.Dto;
using Motivation.Domain.Entities.Kpis;

namespace Motivation.Application.Mappings;

public static class KpiTargetMapper
{
    public static KpiTargetDto? MapToDto(this KpiTarget? source)
    {
        if (source is null)
        {
            return null;
        }

        return new KpiTargetDto {
            Target = source.Target.Value,
            ValidFrom = source.ValidFrom,
            ValidTo = source.ValidTo,
            Fact = source.KpiFact?.MapToDto()
        };
    }
}
