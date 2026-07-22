using Motivation.KpiSalesCalculator.Interfaces;
using System.Linq.Expressions;

namespace Motivation.KpiSalesCalculator.Specifications.Base;

public class BaseSpecification<T> : ISpecification<T>
{
    private readonly Expression<Func<T, bool>> _predicate;

    public BaseSpecification(Expression<Func<T, bool>> predicate)
    {
        _predicate = predicate;
    }

    public Expression<Func<T, bool>> Criteria => _predicate;
    public bool IsSatisfiedBy(T entity) => _predicate.Compile()(entity);
}

public class FalseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria => _ => false;
    public bool IsSatisfiedBy(T entity) => false;
}
