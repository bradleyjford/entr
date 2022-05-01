//HintName: EntrEntityIds.g.cs


namespace Entr.Domain.Tests
{
    using System;
    using Entr.Domain;

    public class UserId
    {
        private readonly Int32 _value;
        private int _hashCode = 0;


        public UserId(Int32 value)
        {
            if (value == default)
            {
                throw new ArgumentException("Specified value cannot be Guid.Empty", nameof(value));
            }

            _value = value;
        }

        protected UserId()
        {
        }

        public override bool Equals(object other)
        {
            var other = obj as UserId;

            if (other is null)
            {
                return false;
            }

            return _value.Equals(other._value);
        }

        public override int GetHashCode()
        {
            if (_hashCode == 0)
            {
                _hashCode = CalculateHashCode();
            }

            return _hashCode;
        }

        private int CalculateHashCode()
        {
            var hashCode = HashCodeUtility.Seed;
            hashCode = HashCodeUtility.Hash(hashCode, GetType());
            hashCode = HashCodeUtility.Hash(hashCode, _value);
            return hashCode;
        }

        public override string ToString()
        {
            return $"<UserId>{_value}";
        }

        public static bool operator ==(UserId left, UserId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UserId left, UserId right)
        {
            return !Equals(left, right);
        }
    }
}


namespace Entr.Domain.Tests
{
    using System;
    using Entr.Domain;

    public class RoleId
    {
        private readonly Guid _value;
        private int _hashCode = 0;


        public static RoleId New()
        {
            return new RoleId(SequentialGuidGenerator.GenerateId());
        }

        public RoleId(Guid value)
        {
            if (value == default)
            {
                throw new ArgumentException("Specified value cannot be Guid.Empty", nameof(value));
            }

            _value = value;
        }

        protected RoleId()
        {
        }

        public override bool Equals(object other)
        {
            var other = obj as RoleId;

            if (other is null)
            {
                return false;
            }

            return _value.Equals(other._value);
        }

        public override int GetHashCode()
        {
            if (_hashCode == 0)
            {
                _hashCode = CalculateHashCode();
            }

            return _hashCode;
        }

        private int CalculateHashCode()
        {
            var hashCode = HashCodeUtility.Seed;
            hashCode = HashCodeUtility.Hash(hashCode, GetType());
            hashCode = HashCodeUtility.Hash(hashCode, _value);
            return hashCode;
        }

        public override string ToString()
        {
            return $"<RoleId>{_value}";
        }

        public static bool operator ==(RoleId left, RoleId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RoleId left, RoleId right)
        {
            return !Equals(left, right);
        }
    }
}

