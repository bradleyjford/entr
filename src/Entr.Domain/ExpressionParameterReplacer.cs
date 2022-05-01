using System.Linq.Expressions;

namespace Entr.Domain;

internal class ExpressionParameterReplacer : ExpressionVisitor
{
    readonly ParameterExpression _parameter;

    internal ExpressionParameterReplacer(ParameterExpression parameter)
    {
        _parameter = parameter;
    }

    protected override Expression VisitParameter(ParameterExpression parameterExpression)
        => base.VisitParameter(_parameter);
}
