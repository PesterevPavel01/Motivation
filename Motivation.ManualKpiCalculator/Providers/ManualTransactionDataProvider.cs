using Motivation.Contracts.Models;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.ManualKpiCalculator.Providers;

public class ManualTransactionDataProvider : ITransactionDataProvider<ManualResultDto>
{

    public async Task<IEnumerable<ManualResultDto>> GetTransactionsAsync(
        DateTime periodFrom,
        DateTime periodTo,
        CancellationToken cancellationToken = default)
    {
        return [];
    }
}
