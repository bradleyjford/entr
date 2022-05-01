using System;

namespace Entr.Domain.Tests;

[EntrEntityId<int>]
public partial class PersonId { }

[EntrEntityId<int>]
public partial class DogId { }

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
