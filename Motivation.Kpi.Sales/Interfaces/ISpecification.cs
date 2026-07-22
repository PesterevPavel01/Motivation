using System.Linq.Expressions;

namespace Motivation.KpiSalesCalculator.Interfaces;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    bool IsSatisfiedBy(T entity);
}
