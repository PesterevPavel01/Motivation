using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.KpiCalculator.Core.Interfaces;
using System.Linq.Expressions;

namespace Motivation.KpiCalculator.Core.Specifications;

public class ProductFilterSpecification : ISpecification<CustomerTransactionProduct>
{
    private readonly HashSet<string> _includeCodes;
    private readonly HashSet<string> _excludeCodes;
    private readonly Func<Product, IEnumerable<string>> _getGroupHierarchy;

    public ProductFilterSpecification(
        List<string> includeCodes,
        List<string> excludeCodes,
        Func<Product, IEnumerable<string>> getGroupHierarchy)
    {
        _includeCodes = includeCodes?.ToHashSet() ?? new HashSet<string>();
        _excludeCodes = excludeCodes?.ToHashSet() ?? new HashSet<string>();
        _getGroupHierarchy = getGroupHierarchy;
    }

    public Expression<Func<CustomerTransactionProduct, bool>> Criteria =>
        ctp => IsSatisfiedBy(ctp);

    public bool IsSatisfiedBy(CustomerTransactionProduct entity)
    {
        var productCodes = _getGroupHierarchy(entity.Product).ToHashSet();

        if (_includeCodes.Any() && !productCodes.Intersect(_includeCodes).Any())
            return false;

        if (_excludeCodes.Any() && productCodes.Intersect(_excludeCodes).Any())
            return false;

        return true;
    }
}
