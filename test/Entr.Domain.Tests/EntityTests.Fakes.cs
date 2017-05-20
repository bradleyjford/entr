using Entr.Domain;

namespace Seed.Common.Tests.Domain
{
    partial class EntityTests
    {
        public class Person : Entity<int>
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

        public class Dog : Entity<int>
        {
            public Dog(int id)
            {
                Id = id;
            }
        }
    }
}
