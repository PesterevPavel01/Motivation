using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.KpiCalculator.Core;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.ReceivablesKpiCalculator;

public class ReceivableKpiQueryBuilder : BaseKpiQueryBuilder<ReceivableDto>
{
    public ReceivableKpiQueryBuilder(
        ITransactionDataProvider<ReceivableDto> dataProvider,
        IEnumerable<IFilter<ReceivableDto>> specifications)
        : base(dataProvider, specifications){}
}
