using Motivation.Contracts.Dto;
using Motivation.Domain.Enum;

namespace Motivation.KpiCalculator.Core.Interfaces;

public interface IFilter<TDto>
    where TDto : class
{
    void Initialize(IEnumerable<KpiFilterDto> filters);
    IEnumerable<TDto> Apply(IEnumerable<TDto> models);
    int Index { get; }
    abstract KpiFilterType FilterType { get; }
}
