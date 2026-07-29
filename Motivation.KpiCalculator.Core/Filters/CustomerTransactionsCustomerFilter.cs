using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto;
using Motivation.Domain.Enum;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiCalculator.Core.Filters;

public class CustomerTransactionsCustomerFilter : IFilter<CustomerTransactionDto>
{
    private HashSet<string> _includeCodes = null!;
    private HashSet<string> _excludeCodes = null!;
    private readonly IHierarchyService _hierarchyService;

    public CustomerTransactionsCustomerFilter(IHierarchyService hierarchyService)
    {
        _hierarchyService = hierarchyService;
    }

    public int Index => 1;
    public KpiFilterType FilterType => KpiFilterType.Customer;

    public void Initialize(IEnumerable<KpiFilterDto> filters)
    {
        if (filters is null || !filters.Any())
        {
            _includeCodes = [];
            _excludeCodes = [];
            return;
        }
        _includeCodes = filters
            .Where(x => x.Include)
            .Select(x => x.Title)
            .ToHashSet();

        _excludeCodes = filters
            .Where(x => !x.Include)
            .Select(x => x.Title)
            .ToHashSet();
    }

    public IEnumerable<CustomerTransactionDto> Apply(IEnumerable<CustomerTransactionDto> models)
    {
        if (_includeCodes is null || _excludeCodes is null)
            throw new ArgumentNullException("Codes is null!");

        if (!_includeCodes.Any() && !_excludeCodes.Any())
            return models;

        var result = models
            .Where(IsSatisfiedBy)
            .ToList();

        return result;
    }

    public bool IsSatisfiedBy(CustomerTransactionDto entity)
    {
        if (_includeCodes is null || _excludeCodes is null)
        {
            throw new ArgumentException($"{typeof(CustomerTransactionsCompanyFilter).Name} not initialize!");
        }

        var customerCodes = _hierarchyService.GetCustomerGroupHierarchy(entity.Customer).ToHashSet();

        if (_includeCodes.Any() && !customerCodes.Intersect(_includeCodes).Any())
            return false;

        if (_excludeCodes.Any() && customerCodes.Intersect(_excludeCodes).Any())
            return false;

        return true;
    }
}
