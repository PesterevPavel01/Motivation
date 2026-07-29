using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto;
using Motivation.KpiCalculator.Core.Interfaces;
using System.Linq.Expressions;
using Motivation.Domain.Enum;

namespace Motivation.KpiCalculator.Core.Filters;

public class CustomerTransactionResponsibleFilter : IFilter<CustomerTransactionDto>
{
    private HashSet<string> _responsibles = null!;

    public CustomerTransactionResponsibleFilter(){}

    public Expression<Func<CustomerTransactionDto, bool>> Criteria =>
        ct => IsSatisfiedBy(ct);

    public int Index => 1;

    public KpiFilterType FilterType => KpiFilterType.SalesResponsible;

    public IEnumerable<CustomerTransactionDto> Apply(IEnumerable<CustomerTransactionDto> models)
    {
        return models
            .Where(IsSatisfiedBy)
            .ToList();
    }

    public void Initialize(IEnumerable<KpiFilterDto> filters)
    {
        if (filters is null || !filters.Any())
        {
            _responsibles = [];
            return;
        }

        Initialize(
            filters
                .Select(x => x.Title)
                .ToList(),
            []);
    }

    public void Initialize(List<string> includeValues, List<string> excludeValues)
    {
        if (includeValues is null || !includeValues.Any())
        {
            _responsibles = [];
        }
        else
        {
            _responsibles = includeValues.ToHashSet();
        }
    }

    public bool IsSatisfiedBy(CustomerTransactionDto model)
    {
        if (!_responsibles.Any())
            return true;

        if (model.Responsible is null)
            return false;

        return _responsibles.Contains(model.Responsible);
    }
}
