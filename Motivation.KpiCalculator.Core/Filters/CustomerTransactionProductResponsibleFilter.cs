using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.Contracts.Dto;
using Motivation.Domain.Enum;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiCalculator.Core.Filters;

public class CustomerTransactionProductResponsibleFilter : IFilter<CustomerTransactionDto>
{
    private HashSet<string> _purchasers = null!;

    public CustomerTransactionProductResponsibleFilter()
    { }

    public int Index => 4;

    public KpiFilterType FilterType => KpiFilterType.ProductResponsible;

    public IEnumerable<CustomerTransactionDto> Apply(IEnumerable<CustomerTransactionDto> models)
    {
        if(_purchasers is null)
            throw new ArgumentNullException("Purchasers is null!");

        if (!_purchasers.Any())
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
            _purchasers = [];
            return;
        }

        _purchasers = filters
            .Select(x => x.Title)
            .ToHashSet();
    }

    private bool IsSatisfiedBy(TransactionProductDto model)
    {
        return 
            model.Product?.Purchaser is not null 
            && _purchasers.Contains(model.Product.Purchaser);
    }
}
