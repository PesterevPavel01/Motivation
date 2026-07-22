using Motivation.Contracts.Dto;
using Motivation.Domain.Entities.Kpis;

namespace Motivation.Application.Mappings;

public static class KpiFactMapper
{
    public static KpiFactDto? MapToDto(this KpiFact? source)
    {
        if (source is null)
        {
            return null;
        }

        return new KpiFactDto
        (
            Value: source.Fact.Value,
            Achievement: source.Achievement.Value,
            BonusPercent: source.BonusPercent.Value,
            BonusAmount: source.BonusAmount.Value
        );
    }
}
