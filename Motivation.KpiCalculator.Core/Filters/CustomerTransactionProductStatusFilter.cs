using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto;
using Motivation.Domain.Enum;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiCalculator.Core.Filters;

internal class CustomerTransactionProductStatusFilter : IFilter<CustomerTransactionDto>
{
    private HashSet<string> _includeStatuses = null!;
    private HashSet<string> _excludeStatuses = null!;

    public CustomerTransactionProductStatusFilter()
    {
    }

    public int Index => 5;

    public KpiFilterType FilterType => KpiFilterType.ProductStatus;

    public IEnumerable<CustomerTransactionDto> Apply(IEnumerable<CustomerTransactionDto> models)
    {
        if (_includeStatuses is null || _excludeStatuses is null)
            throw new ArgumentNullException("Codes is null!");

        if (!_includeStatuses.Any() && !_excludeStatuses.Any())
            return models;

        return models
            .Select(x => x.ToDto(IsSatisfiedBy))
            .Where(x => x.TransactionProducts.Any())
            .ToList();
    }

    public void Initialize(IEnumerable<KpiFilterDto> filters)
    {
        if (filters is null || !filters.Any())
        {
            _includeStatuses = [];
            _excludeStatuses = [];
            return;
        }

        _includeStatuses = filters
            .Where(x => x.Include)
            .Select(x => x.Title)
            .ToHashSet();

        _excludeStatuses = filters
            .Where(x => !x.Include)
            .Select(x => x.Title)
            .ToHashSet();
    }

    public bool IsSatisfiedBy(TransactionProductDto model)
    {
        var status = model.Product?.Status ?? string.Empty;

        if (_includeStatuses.Any() && !_includeStatuses.Contains(status))
            return false;

        if (_excludeStatuses.Any() && _excludeStatuses.Contains(status))
            return false;

        return true;
    }
}
