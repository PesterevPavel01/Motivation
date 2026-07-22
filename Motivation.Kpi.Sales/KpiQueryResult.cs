using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;

namespace Motivation.KpiSalesCalculator;

public class KpiQueryResult
{
    public IEnumerable<CustomerTransactionDto> Transactions { get; set; } = [];
}
