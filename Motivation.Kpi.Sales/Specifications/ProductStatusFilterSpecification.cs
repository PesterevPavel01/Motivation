using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.KpiSalesCalculator.Interfaces;
using System.Linq.Expressions;

namespace Motivation.KpiSalesCalculator.Specifications;

internal class ProductStatusFilterSpecification : ISpecification<CustomerTransactionProduct>
{
    private readonly HashSet<string> _includeStatuses;
    private readonly HashSet<string> _excludeStatuses;

    public ProductStatusFilterSpecification(
        List<string> includeStatuses,
        List<string> excludeStatuses)
    {
        _includeStatuses = includeStatuses?.ToHashSet() ?? new HashSet<string>();
        _excludeStatuses = excludeStatuses?.ToHashSet() ?? new HashSet<string>();
    }

    public Expression<Func<CustomerTransactionProduct, bool>> Criteria =>
        ctp => IsSatisfiedBy(ctp);

    public bool IsSatisfiedBy(CustomerTransactionProduct entity)
    {
        var status = entity.Product?.Status?.Value ?? string.Empty;

        if (_includeStatuses.Any() && !_includeStatuses.Contains(status))
            return false;

        if (_excludeStatuses.Any() && _excludeStatuses.Contains(status))
            return false;

        return true;
    }
}
