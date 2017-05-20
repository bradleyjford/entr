using System;

namespace Entr.Domain
{
    public abstract class Entity<TId> : IEquatable<Entity<TId>>
    {
        readonly object _hashCodeLock = new object();
        int? _hashCode;

        public TId Id { get; protected internal set; }

        public override bool Equals(object obj)
        {
            var other = obj as Entity<TId>;

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            
            return Equals(other);
        }

        public bool Equals(Entity<TId> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            
            if (other.GetType() != GetType()) return false;

            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            if (!_hashCode.HasValue)
            {
                lock (_hashCodeLock)
                {
                    if (!_hashCode.HasValue)
                    {
                        _hashCode = EntityHashCodeCalculator.CalculateHashCode(this);
                    }
                }
            }

            return _hashCode.Value;
        }

        public static bool operator ==(Entity<TId> left, Entity<TId> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Entity<TId> left, Entity<TId> right)
        {
            return !Equals(left, right);
        }
    }
}
