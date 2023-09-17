namespace Entr.Domain;

public interface IId<out TId, TValue>
{
    static abstract TId Create(TValue value);
    TValue Value { get; }
}
