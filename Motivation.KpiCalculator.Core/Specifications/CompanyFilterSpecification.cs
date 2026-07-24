using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.KpiCalculator.Core.Interfaces;
using System.Linq.Expressions;

namespace Motivation.KpiCalculator.Core.Specifications;

public class CompanyFilterSpecification : ISpecification<CustomerTransaction>
{
    private readonly HashSet<string> _includeCodes;
    private readonly HashSet<string> _excludeCodes;

    public CompanyFilterSpecification(List<string> includeCodes, List<string> excludeCodes)
    {
        _includeCodes = includeCodes?.ToHashSet() ?? new HashSet<string>();
        _excludeCodes = excludeCodes?.ToHashSet() ?? new HashSet<string>();
    }

    public Expression<Func<CustomerTransaction, bool>> Criteria =>
        ct => IsSatisfiedBy(ct);

    public bool IsSatisfiedBy(CustomerTransaction entity)
    {
        if (_includeCodes.Any() && !_includeCodes.Contains(entity.Company.Code.Value))
            return false;

        if (_excludeCodes.Any() && _excludeCodes.Contains(entity.Company.Code.Value))
            return false;

        return true;
    }
}