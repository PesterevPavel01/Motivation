using Motivation.KpiSalesCalculator.Interfaces;
using Motivation.KpiSalesCalculator.Specifications.Base;
using System.Linq.Expressions;

namespace Motivation.KpiSalesCalculator.Specifications;

public class SpecificationBuilder<T> : ISpecificationBuilder<T>
{
    public ISpecification<T> Build(Expression<Func<T, bool>> predicate)
    {
        return new ExpressionSpecification<T>(predicate);
    }

    public ISpecification<T> And(ISpecification<T> left, ISpecification<T> right)
    {
        return new AndSpecification<T>(left, right);
    }

    public ISpecification<T> Or(ISpecification<T> left, ISpecification<T> right)
    {
        return new OrSpecification<T>(left, right);
    }

    public ISpecification<T> Not(ISpecification<T> specification)
    {
        return new NotSpecification<T>(specification);
    }
}
