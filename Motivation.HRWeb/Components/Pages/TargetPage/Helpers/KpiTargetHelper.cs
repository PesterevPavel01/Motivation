using Motivation.Contracts.Dto;

namespace Motivation.HRWeb.Components.Pages.TargetPage.Helpers;

public static class KpiTargetHelper
{
    public static decimal? GetTargetForPeriod(
        IEnumerable<KpiTargetDto>? targets,
        int year,
        int month)
    {
        if (targets is null || !targets.Any())
            return null;

        var periodStart = new DateTime(year, month, 1);
        var periodEnd = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);

        var target = targets.FirstOrDefault(t =>
            t.ValidFrom.Date == periodStart.Date &&
            t.ValidTo.Date == periodEnd.Date);

        return target?.Target;
    }
}
