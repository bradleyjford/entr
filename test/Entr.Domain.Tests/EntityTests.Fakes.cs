using System;

namespace Entr.Domain.Tests;

[EntityId<int>]
public partial struct PersonId { }

[EntityId<int>]
public partial struct DogId { }

partial class EntityTests
{
    public class Person : Entity<PersonId>
    {
        public Person(PersonId id)
        {
            Id = id;
        }

        public void SetId(PersonId id)
        {
            Id = id;
        }
    }

    public class Dog : Entity<DogId>
    {
        public Dog(DogId id)
        {
            Id = id;
        }
    }
}
