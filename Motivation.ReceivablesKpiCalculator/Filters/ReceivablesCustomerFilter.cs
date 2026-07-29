using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto;
using Motivation.Domain.Enum;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.ReceivablesKpiCalculator.Filters;

public class ReceivablesCustomerFilter : IFilter<ReceivableDto>
{
    private HashSet<string> _includeCodes = null!;
    private HashSet<string> _excludeCodes = null!;
    private readonly IHierarchyService _hierarchyService;

    public ReceivablesCustomerFilter(IHierarchyService hierarchyService)
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

    public IEnumerable<ReceivableDto> Apply(IEnumerable<ReceivableDto> models)
    {
        if (_includeCodes is null || _excludeCodes is null)
            throw new ArgumentNullException("Codes is null!");

        if (!_includeCodes.Any() && !_excludeCodes.Any())
            return models;

        var result = models
            .GroupBy(x => x.Customer!.MasterCustomer!.Code)
            .Select(g => new ReceivableDto
            {
                Customer = g.First().Customer!.MasterCustomer!,
                Amount = g.Sum(x => x.Amount),
                AsOfDate = g.First().AsOfDate,
                Company = g.First().Company
            })
            .Where(IsSatisfiedBy)
            .ToList();

        return result;
    }

    public bool IsSatisfiedBy(ReceivableDto entity)
    {
        if (_includeCodes is null || _excludeCodes is null)
        {
            throw new ArgumentException($"{typeof(ReceivablesCustomerFilter).Name} not initialize!");
        }

        if (entity.Amount >= 0)
            return false;

        var customerCodes = _hierarchyService.GetCustomerGroupHierarchy(entity.Customer).ToHashSet();

        if (_includeCodes.Any() && !customerCodes.Intersect(_includeCodes).Any())
            return false;

        if (_excludeCodes.Any() && customerCodes.Intersect(_excludeCodes).Any())
            return false;

        return true;
    }
}
