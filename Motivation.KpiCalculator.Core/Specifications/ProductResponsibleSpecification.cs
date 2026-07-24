using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;
using Motivation.KpiCalculator.Core.Interfaces;
using System.Linq.Expressions;

namespace Motivation.KpiCalculator.Core.Specifications;

public class ProductResponsibleSpecification : ISpecification<CustomerTransaction>
{
    private readonly HashSet<PurchaserValue> _purchaserValues;

    public ProductResponsibleSpecification(List<PurchaserValue> purchaserValues)
    {
        _purchaserValues = purchaserValues?.ToHashSet() ?? new HashSet<PurchaserValue>();
    }

    public Expression<Func<CustomerTransaction, bool>> Criteria =>
        ct => IsSatisfiedBy(ct);

    public bool IsSatisfiedBy(CustomerTransaction entity)
    {
        if (!_purchaserValues.Any())
            return true;

        return entity.CustomerTransactionProducts
            .Any(ctp => ctp.Product?.Purchaser is not null &&
                        _purchaserValues.Contains(ctp.Product.Purchaser));
    }
}
