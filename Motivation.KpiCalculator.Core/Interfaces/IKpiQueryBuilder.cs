using Motivation.Contracts.Dto;

namespace Motivation.KpiCalculator.Core.Interfaces;

public interface IKpiQueryBuilder<TDto>
    where TDto : class
{
    IKpiQueryBuilder<TDto> SetPeriod(DateTime from, DateTime to);
    IKpiQueryBuilder<TDto> InitializeFilters(IEnumerable<KpiFilterDto> filters);
    Task<KpiQueryResult<TDto>> BuildAsync(CancellationToken cancellationToken = default);
}
