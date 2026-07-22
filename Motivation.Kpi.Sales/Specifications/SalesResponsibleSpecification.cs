using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;
using Motivation.KpiSalesCalculator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Motivation.KpiSalesCalculator.Specifications;

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
