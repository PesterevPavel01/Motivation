using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto;
using Motivation.Domain.Enum;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiCalculator.Core.Filters;

public class CustomerTransactionCustomerManagerFilter : IFilter<CustomerTransactionDto>
{
    private HashSet<string> _managers = null!;

    public CustomerTransactionCustomerManagerFilter(){}

    public int Index => 0;

    public KpiFilterType FilterType => KpiFilterType.CustomerResponsible;

    public IEnumerable<CustomerTransactionDto> Apply(IEnumerable<CustomerTransactionDto> models)
    {
        if(_managers is null)
            throw new ArgumentNullException("Managers is null!");

        if (!_managers.Any())
            return models;

        return models
            .Where(IsSatisfiedBy)
            .ToList();
    }

    public void Initialize(IEnumerable<KpiFilterDto> filters)
    {
        if (filters is null || !filters.Any())
        {
            _managers = [];
            return;
        }

        _managers = filters
            .Select(x => x.Title)
            .ToHashSet();
    }

    public bool IsSatisfiedBy(CustomerTransactionDto entity)
    {
        if (_managers is null)
            return true;

        if (!_managers.Any())
            return true;

        if (entity.Customer?.Manager is null)
            return false;

        return _managers.Contains(entity.Customer.Manager);
    }
}