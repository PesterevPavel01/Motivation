using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto;
using Motivation.Domain.Enum;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.ReceivablesKpiCalculator.Filters;

public class ReceivablesCompanyFilter : IFilter<ReceivableDto>
{
    private HashSet<string> _includeCodes = null!;
    private HashSet<string> _excludeCodes = null!;

    public int Index => 0;
    public KpiFilterType FilterType => KpiFilterType.Company;

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
            .Where(IsSatisfiedBy)
            .ToList();

        return result;
    }

    public bool IsSatisfiedBy(ReceivableDto entity)
    {
        if (_includeCodes.Any() && !_includeCodes.Contains(entity.Company.Code))
            return false;

        if (_excludeCodes.Any() && _excludeCodes.Contains(entity.Company.Code))
            return false;

        return true;
    }
}