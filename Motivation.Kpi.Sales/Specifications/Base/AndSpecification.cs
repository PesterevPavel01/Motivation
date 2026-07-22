using Motivation.KpiSalesCalculator.Interfaces;
using System.Linq.Expressions;

namespace Motivation.KpiSalesCalculator.Specifications.Base;

public class AndSpecification<T> : ISpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;
    }

    public Expression<Func<T, bool>> Criteria
    {
        get
        {
            var leftExpr = _left.Criteria;
            var rightExpr = _right.Criteria;

            var param = Expression.Parameter(typeof(T));
            var leftBody = ReplaceParameter(leftExpr.Body, leftExpr.Parameters[0], param);
            var rightBody = ReplaceParameter(rightExpr.Body, rightExpr.Parameters[0], param);

            var andExpression = Expression.AndAlso(leftBody, rightBody);
            return Expression.Lambda<Func<T, bool>>(andExpression, param);
        }
    }

    public bool IsSatisfiedBy(T entity) => _left.IsSatisfiedBy(entity) && _right.IsSatisfiedBy(entity);

    private Expression ReplaceParameter(Expression body, ParameterExpression oldParam, ParameterExpression newParam)
    {
        return new ParameterReplacer(oldParam, newParam).Visit(body);
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParam;
        private readonly ParameterExpression _newParam;

        public ParameterReplacer(ParameterExpression oldParam, ParameterExpression newParam)
        {
            _oldParam = oldParam;
            _newParam = newParam;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParam ? _newParam : base.VisitParameter(node);
        }
    }
}
