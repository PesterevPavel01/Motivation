namespace Motivation.KpiCalculator.Core.Interfaces;

public interface ITransactionDataProvider<TDto>
{
    Task<IEnumerable<TDto>> GetTransactionsAsync(
        DateTime periodFrom,
        DateTime periodTo,
        CancellationToken cancellationToken = default);
}
