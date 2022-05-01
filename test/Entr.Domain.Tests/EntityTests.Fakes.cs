using System;

namespace Entr.Domain.Tests
{
    partial class EntityTests
    {
        class Person : Entity<int>
        {
            public Person(int id)
            {
                Id = id;
            }

            public void SetId(int id)
            {
                Id = id;
            }
        }

        class Dog : Entity<int>
        {
            public Dog(int id)
            {
                Id = id;
            }
        }
    }
}
