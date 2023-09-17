using JetBrains.Annotations;

namespace Entr.Domain;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
{
    readonly object _hashCodeLock = new();
    volatile int _hashCode;

#pragma warning disable CS0414
#pragma warning disable CA1823
    byte[] _rowVersion = default!;
#pragma warning restore CA1823
#pragma warning restore CS0414

    public TId Id { get; protected internal set; } = default!;

    public override bool Equals(object? obj)
    {
        var other = obj as Entity<TId>;

        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Equals(other);
    }

    public bool Equals(Entity<TId>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        if (other.GetType() != GetType()) return false;

        return Id!.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        if (_hashCode == 0)
        {
            lock (_hashCodeLock)
            {
                if (_hashCode == 0)
                {
                    _hashCode = EntityHashCodeCalculator.CalculateHashCode(this);
                }
            }
        }

        return _hashCode;
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
        => Equals(left, right);
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
        => !Equals(left, right);
}
