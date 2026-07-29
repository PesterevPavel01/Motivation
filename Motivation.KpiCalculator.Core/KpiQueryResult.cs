using Motivation.AnalyticsCore.Contracts.Models;

namespace Motivation.KpiCalculator.Core;

public class KpiQueryResult<TDto>
    where TDto : class
{
    public IEnumerable<TDto> Transactions { get; set; } = [];
}
