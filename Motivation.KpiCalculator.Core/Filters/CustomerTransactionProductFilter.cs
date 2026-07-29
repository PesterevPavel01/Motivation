using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto;
using Motivation.KpiCalculator.Core.Interfaces;
using System.Linq.Expressions;
using Motivation.Domain.Enum;

namespace Motivation.KpiCalculator.Core.Filters;

public class CustomerTransactionProductFilter : IFilter<CustomerTransactionDto>
{
    private HashSet<string> _includeCodes = null!;
    private HashSet<string> _excludeCodes = null!;
    private readonly IHierarchyService _hierarchyService;

    public int Index => 3;

    public Expression<Func<CustomerTransactionDto, bool>> Criteria => throw new NotImplementedException();

    public KpiFilterType FilterType => KpiFilterType.Product;

    public CustomerTransactionProductFilter(IHierarchyService hierarchyService)
    {
        _hierarchyService = hierarchyService;
    }

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

        if(!_includeCodes.Any() && !_excludeCodes.Any())
            return models;

        var result = models
            .Select(x => x.ToDto(IsSatisfiedBy))
            .ToList();

        return result;
    }

    private bool IsSatisfiedBy(TransactionProductDto model)
    {
        var productCodes = _hierarchyService.GetProductGroupHierarchy(model.Product).ToHashSet();

        if (_includeCodes.Any() && !productCodes.Intersect(_includeCodes).Any())
            return false;

        if (_excludeCodes.Any() && productCodes.Intersect(_excludeCodes).Any())
            return false;

        return true;
    }
}
