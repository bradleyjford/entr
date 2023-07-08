using System.Diagnostics.CodeAnalysis;

namespace Entr.Domain;

public abstract class Maybe<T>
{
    public static readonly None<T> None = new();

    public static Maybe<T> Some(T value)
    {
        return new Some<T>(value);
    }

    public bool TryGetValue([NotNullWhen(true)] out T value)
    {
        if (this is Some<T> some)
        {
            value = some.Value!;
            return true;
        }

        value = default!;
        return false;
    }

    public static implicit operator Maybe<T>(T? value)
    {
        if(value == null)
            return None;

        return new Some<T>(value);
    }
}

public sealed class Some<T> : Maybe<T>
{
    public Some(T value) => Value = value;
    public T Value { get; }
}

public sealed class None<T> : Maybe<T>
{
}
