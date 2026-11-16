using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Models;
using Motivation.KpiCalculator.Core;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.ManualKpiCalculator;

public class ManualKpiQueryBuilder : BaseKpiQueryBuilder<ManualResultDto>
{
    public ManualKpiQueryBuilder(
        ITransactionDataProvider<ManualResultDto> dataProvider,
        IEnumerable<IFilter<ManualResultDto>> specifications)
        : base(dataProvider, specifications){}
}
