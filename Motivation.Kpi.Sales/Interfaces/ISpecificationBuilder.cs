using System.Linq.Expressions;

namespace Motivation.KpiSalesCalculator.Interfaces;

public interface ISpecificationBuilder<T>
{
    ISpecification<T> Build(Expression<Func<T, bool>> predicate);
    ISpecification<T> And(ISpecification<T> left, ISpecification<T> right);
    ISpecification<T> Or(ISpecification<T> left, ISpecification<T> right);
    ISpecification<T> Not(ISpecification<T> specification);
}