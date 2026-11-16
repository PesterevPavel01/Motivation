using Motivation.Contracts.Dto;
using Motivation.HRWeb.Components.Pages.HomePage.Models;

namespace Motivation.HRWeb.Application.Mappings;

public static class KpiRowModelMapper
{
    public static KpiRowModel ToKpiRowModel(
        this PositionKpiDto kpi,
        KpiTargetDto? target,
        decimal motivationPart)
    {
        var fact = target?.Fact;
        var weight = kpi.Weight;
        var bonusPercent = fact?.BonusPercent ?? 0;

        return new KpiRowModel
        {
            Type = kpi.Kpi?.KpiType.ToString() ?? string.Empty,
            OrderNumber = kpi.OrderNumber,
            KpiTitle = kpi.KpiTitle ?? string.Empty,
            MeasurementUnit = kpi.Kpi?.MeasurementUnit?.Title ?? string.Empty,
            TargetId = target?.Id,
            Target = target?.Target ?? 0,
            MotivationPart = motivationPart * weight,
            Fact = fact?.Value ?? 0,
            Achievement = fact?.Achievement ?? 0,
            BonusPercent = fact?.BonusPercent ?? 0,
            Bonus = fact?.BonusAmount ?? 0
        };
    }

    public static IEnumerable<KpiRowModel> ToKpiRowModels(
        this IEnumerable<PositionKpiDto> kpis,
        EmployeePositionDto position,
        int month,
        int year,
        decimal motivationPart)
    {
        var periodStart = new DateTime(year, month, 1, 0, 0, 0);
        var periodEnd = new DateTime(year, month, DateTime.DaysInMonth(year, month), 0, 0, 0);

        return kpis
            .Where(kpi => kpi.ValidFrom <= periodEnd && kpi.ValidTo >= periodStart)
            .Select(kpi =>
            {
                var target = kpi.Targets?
                    .FirstOrDefault(t => t.ValidFrom <= periodEnd && t.ValidTo >= periodStart);

                return kpi.ToKpiRowModel(target, motivationPart);
            })
            .OrderBy(row => row.OrderNumber)
            .ToList();
    }
}
