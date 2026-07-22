using Motivation.AnalyticsCore.Domain.Entities;

namespace Motivation.KpiSalesCalculator.Interfaces;

public interface ITransactionDataProvider
{
    Task<IEnumerable<CustomerTransaction>> GetTransactionsAsync(
        DateTime periodFrom,
        DateTime periodTo,
        CancellationToken cancellationToken = default);
}
