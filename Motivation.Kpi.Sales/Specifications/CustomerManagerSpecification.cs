using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;
using Motivation.KpiSalesCalculator.Interfaces;
using System.Linq.Expressions;

namespace Motivation.KpiSalesCalculator.Specifications;

public class CustomerManagerSpecification : ISpecification<CustomerTransaction>
{
    private readonly HashSet<ManagerValue> _managerValues;

    public CustomerManagerSpecification(List<ManagerValue> managerValues)
    {
        _managerValues = managerValues?.ToHashSet() ?? new HashSet<ManagerValue>();
    }

    public Expression<Func<CustomerTransaction, bool>> Criteria =>
        ct => IsSatisfiedBy(ct);

    public bool IsSatisfiedBy(CustomerTransaction entity)
    {
        if (!_managerValues.Any())
            return true;

        if (entity.Customer?.Manager is null)
            return false;

        return _managerValues.Contains(entity.Customer.Manager);
    }
}