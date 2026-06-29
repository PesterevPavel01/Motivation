using Motivation.Application.Mappings;
using Motivation.Domain.Enum;

namespace Motivation.HRWeb.Components.Pages.PositionsPage.Models;

public class WorkWeekTypeModel
{
    public string Value { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    public static IEnumerable<WorkWeekTypeModel> GetWeekTypes()
    {
        return Enum.GetValues<WorkWeekType>()
            .Select(type => new WorkWeekTypeModel
            {
                Value = type.ToRussianString(),
                DisplayName = type.ToRussianString()
            });
    }
}