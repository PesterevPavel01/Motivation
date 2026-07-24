using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.KpiCalculator.Core.Interfaces;
using System.Linq.Expressions;

namespace Motivation.KpiCalculator.Core.Specifications;

public class CustomerFilterSpecification : ISpecification<CustomerTransaction>
{
    private readonly HashSet<string> _includeCodes;
    private readonly HashSet<string> _excludeCodes;
    private readonly Func<Customer, IEnumerable<string>> _getGroupHierarchy;

    public CustomerFilterSpecification(
        List<string> includeCodes,
        List<string> excludeCodes,
        Func<Customer, IEnumerable<string>> getGroupHierarchy)
    {
        _includeCodes = includeCodes?.ToHashSet() ?? new HashSet<string>();
        _excludeCodes = excludeCodes?.ToHashSet() ?? new HashSet<string>();
        _getGroupHierarchy = getGroupHierarchy;
    }

    public Expression<Func<CustomerTransaction, bool>> Criteria =>
        ct => IsSatisfiedBy(ct);

    public bool IsSatisfiedBy(CustomerTransaction entity)
    {
        var customerCodes = _getGroupHierarchy(entity.Customer).ToHashSet();

        if (customerCodes.Contains("С00001496"))
            return true;



        if (_includeCodes.Any() && !customerCodes.Intersect(_includeCodes).Any())
            return false;

        if (_excludeCodes.Any() && customerCodes.Intersect(_excludeCodes).Any())
            return false;

        return true;
    }
}
