using System;
using Xunit;

namespace Seed.Common.Tests.Domain
{
    public partial class EntityTests
    {
        [Fact]
        public void Equals_EntitiesOfSameTypeWithSameId_AreEqual()
        {
            var a = new Person(5);
            var b = new Person(5);

            Assert.True(a.Equals(b));
        }

        [Fact]
        public void Equals_EntitiesOfSameTypeWithDifferentIds_AreNotEqual()
        {
            var a = new Person(1);
            var b = new Person(2);

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void Equals_ComparingEntityWithNull_IsNotEqual()
        {
            var a = new Person(1);
            
            Assert.False(a.Equals(null));
        }

        [Fact]
        public void Equals_EntitiesOfDifferentTypesWithSameIds_AreNotEqual()
        {
            var person = new Person(1);
            var dog = new Dog(1);

// ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(person.Equals(dog));
        }

        [Fact]
        public void Equals_AnEntity_IsEqualToItself()
        {
            var person = new Person(1);

            Assert.True(person.Equals(person));
        }

        [Fact]
        public void EqualsOperator_EntitiesOfSameTypeWithSameId_AreEqual()
        {
            var a = new Person(5);
            var b = new Person(5);

            Assert.True(a == b);
        }

        [Fact]
        public void EqualsOperator_EntitiesOfSameTypeWithDifferingIds_AreNotEqual()
        {
            var a = new Person(1);
            var b = new Person(5);

            Assert.False(a == b);
        }

        [Fact]
        public void NotEqualsOperator_EntitiesOfSameTypeWithSameId_AreEqual()
        {
            var a = new Person(5);
            var b = new Person(5);

            Assert.False(a != b);
        }

        [Fact]
        public void NotEqualsOperator_EntitiesOfSameTypeWithDifferingIds_AreNotEqual()
        {
            var a = new Person(1);
            var b = new Person(5);

            Assert.True(a != b);
        }

        [Fact]
        public void GetHashCode_EntitiesOfSameTypeWithSameId_HaveSameHashCode()
        {
            var a = new Person(5);
            var b = new Person(5);

            Assert.True(a.GetHashCode() == b.GetHashCode());
        }

        [Fact]
        public void GetHashCode_EntitiesOfSameTypeWithDifferingIds_HaveDifferentHashCodes()
        {
            var a = new Person(1);
            var b = new Person(5);

            Assert.True(a.GetHashCode() != b.GetHashCode());
        }

        [Fact]
        public void GetHashCode_ChangingAnEntitiesId_DoesNotChangeHashCode()
        {
            var person = new Person(0);

            var originalHashCode = person.GetHashCode();

            person.SetId(5);

            var newHashCode = person.GetHashCode();

            Assert.Equal(originalHashCode, newHashCode);
        }
    }
}
