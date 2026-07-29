using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.KpiCalculator.Core;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiSalesCalculator;

public class SalesKpiQueryBuilder : BaseKpiQueryBuilder<CustomerTransactionDto>
{
    public SalesKpiQueryBuilder(
        ITransactionDataProvider<CustomerTransactionDto> dataProvider,
        IEnumerable<IFilter<CustomerTransactionDto>> specifications)
        : base(dataProvider, specifications)
    {
    }
}
