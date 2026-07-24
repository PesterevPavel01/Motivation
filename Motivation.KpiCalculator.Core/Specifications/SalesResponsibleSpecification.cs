using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;
using Motivation.KpiCalculator.Core.Interfaces;
using System.Linq.Expressions;

namespace Motivation.KpiCalculator.Core.Specifications;

public class SalesResponsibleSpecification : ISpecification<CustomerTransaction>
{
    private readonly HashSet<ResponsibleValue> _responsibleValues;

    public SalesResponsibleSpecification(List<ResponsibleValue> responsibleValues)
    {
        _responsibleValues = responsibleValues?.ToHashSet() ?? new HashSet<ResponsibleValue>();
    }

    public Expression<Func<CustomerTransaction, bool>> Criteria =>
        ct => IsSatisfiedBy(ct);

    public bool IsSatisfiedBy(CustomerTransaction entity)
    {
        if (!_responsibleValues.Any())
            return true;

        if (entity.Responsible is null)
            return false;

        return _responsibleValues.Contains(entity.Responsible);
    }
}
