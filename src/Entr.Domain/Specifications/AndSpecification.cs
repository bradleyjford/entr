using System;
using System.Linq.Expressions;

namespace Entr.Domain.Specifications;
internal sealed class AndSpecification<T> : Specification<T>
{
    public AndSpecification(Specification<T> left, Specification<T> right)
        : base(CreateExpression(left, right))
    {
    }

    static Expression<Func<T, bool>> CreateExpression(Specification<T> left, Specification<T> right)
    {
        var operand1 = left.ToExpression().Body;
        var operand2 = right.ToExpression().Body;

        var parameter = Expression.Parameter(typeof(T));

        var body = Expression.AndAlso(operand1, operand2);
        body = (BinaryExpression)new ExpressionParameterReplacer(parameter).Visit(body);

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}
