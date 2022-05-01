using System;

namespace Entr.Domain;

public abstract class Result
{
    public static readonly EmptyResult Empty = new EmptyResult();

    public static Result Create<TValue>(TValue value)
    {
        return new ValueResult<TValue>(value);
    }

    public virtual bool HasValue { get; } = false;
}

public sealed class EmptyResult : Result
{
    internal EmptyResult()
    {
    }
}

public sealed class ValueResult<TValue> : Result
{
    internal ValueResult(TValue value)
    {
        Value = value;
    }

    public override bool HasValue => true;

    public TValue Value { get; }
}
