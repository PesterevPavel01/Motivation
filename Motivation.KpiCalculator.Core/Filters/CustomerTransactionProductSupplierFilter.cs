using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto;
using Motivation.Domain.Enum;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiCalculator.Core.Filters;

internal class CustomerTransactionProductSupplierFilter : IFilter<CustomerTransactionDto>
{
    private HashSet<string> _includeSupplier = null!;
    private HashSet<string> _excludeSupplier = null!;

    public CustomerTransactionProductSupplierFilter(){}

    public int Index => 6;

    public KpiFilterType FilterType => KpiFilterType.Supplier;

    public void Initialize(IEnumerable<KpiFilterDto> filters)
    {
        if (filters is null || !filters.Any())
        {
            _includeSupplier = [];
            _excludeSupplier = [];
            return;
        }

        _includeSupplier = filters
            .Where(x => x.Include)
            .Select(x => x.Title)
            .ToHashSet();

        _excludeSupplier = filters
            .Where(x => !x.Include)
            .Select(x => x.Title)
            .ToHashSet();
    }

    public IEnumerable<CustomerTransactionDto> Apply(IEnumerable<CustomerTransactionDto> models)
    {
        if (_includeSupplier is null || _excludeSupplier is null)
            throw new ArgumentNullException("Supplier is null!");

        if (!_includeSupplier.Any() && !_excludeSupplier.Any())
            return models;

        return models
            .Select(x => x.ToDto(IsSatisfiedBy))
            .Where(x => x.TransactionProducts.Any())
            .ToList();
    }

    public bool IsSatisfiedBy(TransactionProductDto model)
    {
        var supplier = model.Product?.Supplier ?? string.Empty;

        if (_includeSupplier.Any() && !_includeSupplier.Contains(supplier))
            return false;

        if (_excludeSupplier.Any() && _excludeSupplier.Contains(supplier))
            return false;

        return true;
    }
}