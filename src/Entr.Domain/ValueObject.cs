using System;

namespace Entr.Domain 
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        public abstract bool Equals(ValueObject other);
        public abstract override int GetHashCode();

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            
            if (other.GetType() != GetType()) return false;

            return Equals((ValueObject)other);
        }
    }
}
