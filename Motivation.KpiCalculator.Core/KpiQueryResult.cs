using Motivation.AnalyticsCore.Contracts.Models;

namespace Motivation.KpiCalculator.Core;

public class KpiQueryResult
{
    public IEnumerable<CustomerTransactionDto> Transactions { get; set; } = [];
}
