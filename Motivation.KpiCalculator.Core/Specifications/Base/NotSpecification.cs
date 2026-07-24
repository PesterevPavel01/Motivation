using Motivation.KpiCalculator.Core.Interfaces;
using System.Linq.Expressions;

namespace Motivation.KpiSalesCalculator.Specifications.Base;

public class NotSpecification<T> : ISpecification<T>
{
    private readonly ISpecification<T> _specification;

    public NotSpecification(ISpecification<T> specification)
    {
        _specification = specification;
    }

    public Expression<Func<T, bool>> Criteria
    {
        get
        {
            var expr = _specification.Criteria;
            var param = expr.Parameters[0];
            var notExpression = Expression.Not(expr.Body);
            return Expression.Lambda<Func<T, bool>>(notExpression, param);
        }
    }

    public bool IsSatisfiedBy(T entity) => !_specification.IsSatisfiedBy(entity);
}
