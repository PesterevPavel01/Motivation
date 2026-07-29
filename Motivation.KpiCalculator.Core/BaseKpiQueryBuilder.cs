using Motivation.Contracts.Dto;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiCalculator.Core;

public abstract class BaseKpiQueryBuilder<TDto> : IKpiQueryBuilder<TDto>
    where TDto : class
{
    private readonly ITransactionDataProvider<TDto> _dataProvider;
    private readonly IEnumerable<IFilter<TDto>> _specificSpecifications;

    private DateTime _periodFrom;
    private DateTime _periodTo;

    public BaseKpiQueryBuilder(
        ITransactionDataProvider<TDto> dataProvider,
        IEnumerable<IFilter<TDto>> specificSpecifications)
    {
        _dataProvider = dataProvider;
        _specificSpecifications = specificSpecifications;
    }

    public virtual IKpiQueryBuilder<TDto> SetPeriod(DateTime from, DateTime to)
    {
        _periodFrom = from;
        _periodTo = to;
        return this;
    }

    public virtual IKpiQueryBuilder<TDto> InitializeFilters(IEnumerable<KpiFilterDto> filters)
    {
        foreach (var specification in _specificSpecifications.OrderBy(x => x.Index))
        {
            if (filters is null || !filters.Any())
            {
                specification.Initialize([]);
                return this;
            }
            specification.Initialize(filters.Where(x => x.Type == specification.FilterType).ToList());
        }
        return this;
    }

    public async Task<KpiQueryResult<TDto>> BuildAsync(CancellationToken cancellationToken = default)
    {
        var transactions = await _dataProvider.GetTransactionsAsync(
            _periodFrom,
            _periodTo,
            cancellationToken);

        var filteredTransactions = transactions.AsEnumerable();

        var resultList = ApplyFilters(filteredTransactions);

        return new KpiQueryResult<TDto>
        {
            Transactions = resultList
        };
    }

    private IEnumerable<TDto> ApplyFilters(IEnumerable<TDto> transactions)
    {
        var result = transactions;

        foreach (var specification in _specificSpecifications.OrderBy(x => x.Index))
        {
            result = specification.Apply(result);
        }
        return result;
    }
}