using Motivation.Contracts.Dto;

namespace Motivation.HRWeb.Components.Pages.HomePage.Helpers;

public static class HomeHelper
{
    public static decimal? GetActualHours(IEnumerable<ActualHoursDto>? hours, int month, int year)
    {
        return hours?.FirstOrDefault(h => h.Month == month && h.Year == year)?.Value;
    }

    public static decimal? GetExtraPart(IEnumerable<ExtraPartDto>? extraParts, int month, int year)
    {
        var date = new DateTime(year, month, 1);
        return extraParts?.FirstOrDefault(e => e.ValidFrom <= date && e.ValidTo >= date)?.Value;
    }
    public static decimal CalculateSalary(decimal baseSalary, decimal? actualHours, decimal? standardHours)
    {
        if (standardHours == null || standardHours == 0 || actualHours == null)
            return 0;
        return baseSalary / standardHours.Value * actualHours.Value;
    }
}
