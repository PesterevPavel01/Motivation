using Motivation.KpiCalculator.Core.Interfaces;
using System.Linq.Expressions;

namespace Motivation.KpiCalculator.Core.Specifications;

public class TrueSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria => _ => true;
    public bool IsSatisfiedBy(T entity) => true;
}

