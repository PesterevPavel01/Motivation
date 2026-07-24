using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.KpiCalculator.Core.Interfaces;
using System.Linq.Expressions;

namespace Motivation.KpiCalculator.Core.Specifications;

internal class ProductSupplierFilterSpecification : ISpecification<CustomerTransactionProduct>
{
    private readonly HashSet<string> _includeSupplier;
    private readonly HashSet<string> _excludeSupplier;

    public ProductSupplierFilterSpecification(
        List<string> includeSupplier,
        List<string> excludeSupplier)
    {
        _includeSupplier = includeSupplier?.ToHashSet() ?? new HashSet<string>();
        _excludeSupplier = excludeSupplier?.ToHashSet() ?? new HashSet<string>();
    }

    public Expression<Func<CustomerTransactionProduct, bool>> Criteria =>
        ctp => IsSatisfiedBy(ctp);

    public bool IsSatisfiedBy(CustomerTransactionProduct entity)
    {
        var supplier = entity.Product?.Supplier?.Value ?? string.Empty;

        if (_includeSupplier.Any() && !_includeSupplier.Contains(supplier))
            return false;

        if (_excludeSupplier.Any() && _excludeSupplier.Contains(supplier))
            return false;

        return true;
    }
}