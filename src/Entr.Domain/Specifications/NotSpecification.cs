using System;
using System.Linq.Expressions;

namespace Entr.Domain.Specifications;

internal sealed class NotSpecification<T> : Specification<T>
{
    public NotSpecification(Specification<T> specification)
        : base(CreateExpression(specification))
    {
    }

    static Expression<Func<T, bool>> CreateExpression(Specification<T> specification)
    {
        var operand = specification.ToExpression().Body;

        var parameter = Expression.Parameter(typeof(T));

        var body = Expression.Not(operand);
        body = (UnaryExpression)new ExpressionParameterReplacer(parameter).Visit(body)!;

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}
