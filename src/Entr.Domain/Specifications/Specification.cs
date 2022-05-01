using System;
using System.Linq.Expressions;

namespace Entr.Domain.Specifications
{
    public abstract class Specification<T>
    {
        readonly Expression<Func<T, bool>> _expression;
        readonly Lazy<Func<T, bool>> _delegate;

        protected Specification(Expression<Func<T, bool>> expression)
        {
            _expression = expression;
            _delegate = new Lazy<Func<T, bool>>(() => _expression.Compile());
        }

        public bool IsSatisfiedBy(T value) => _delegate.Value(value);

        public Expression<Func<T, bool>> ToExpression() => _expression;

        public Specification<T> And(Specification<T> other) => new AndSpecification<T>(this, other);
        public Specification<T> Or(Specification<T> other) => new OrSpecification<T>(this, other);
        public Specification<T> Not() => new NotSpecification<T>(this);
    }
}
