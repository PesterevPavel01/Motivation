using System.Linq.Expressions;

namespace Motivation.KpiCalculator.Core.Interfaces;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    bool IsSatisfiedBy(T entity);
}
