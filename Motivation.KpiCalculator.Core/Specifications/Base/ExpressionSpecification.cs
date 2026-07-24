using Motivation.KpiCalculator.Core.Interfaces;
using System.Linq.Expressions;

namespace Motivation.KpiSalesCalculator.Specifications.Base;

public class ExpressionSpecification<T> : ISpecification<T>
{
    private readonly Expression<Func<T, bool>> _predicate;

    public ExpressionSpecification(Expression<Func<T, bool>> predicate)
    {
        _predicate = predicate;
    }

    public Expression<Func<T, bool>> Criteria => _predicate;

    public bool IsSatisfiedBy(T entity)
    {
        return _predicate.Compile()(entity);
    }
}
