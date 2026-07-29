using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto;
using Motivation.Domain.Enum;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiCalculator.Core.Filters;

public class ReceivablesCustomerManagerFilter : IFilter<ReceivableDto>
{
    private HashSet<string> _managers = null!;

    public ReceivablesCustomerManagerFilter(){}

    public int Index => 2;

    public KpiFilterType FilterType => KpiFilterType.CustomerResponsible;

    public IEnumerable<ReceivableDto> Apply(IEnumerable<ReceivableDto> models)
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

    public bool IsSatisfiedBy(ReceivableDto entity)
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