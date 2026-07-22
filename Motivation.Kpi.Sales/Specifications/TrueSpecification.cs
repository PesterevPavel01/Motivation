using Motivation.KpiSalesCalculator.Interfaces;
using System.Linq.Expressions;

namespace Motivation.KpiSalesCalculator.Specifications;

public class TrueSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria => _ => true;
    public bool IsSatisfiedBy(T entity) => true;
}

