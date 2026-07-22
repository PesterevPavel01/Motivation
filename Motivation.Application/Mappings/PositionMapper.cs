using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;

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
            WorkWeekType: source.WorkWeekType.ToRussianString(),
            Kpis: source.PositionKpiHistory
                .Select(x => new PositionKpiDto
                {
                    KpiCode = x.Kpi.Code.Value,
                    KpiTitle = x.Kpi.Title.Value,
                    OrderNumber = x.OrderNumber,
                    Weight = x.Weight.Value,
                    ValidFrom = x.ValidFrom,
                    ValidTo = x.ValidTo,
                    Targets = x.TargetHistory!
                        .Select(t => t.MapToDto())
                        .Where(d => d is not null)
                        .ToList()!,
                    Kpi = x.Kpi?.MapToDto()
                })
                .ToList());
    }
}
