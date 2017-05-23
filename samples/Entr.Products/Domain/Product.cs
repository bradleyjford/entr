using System;
using Entr.Domain;

namespace Entr.Products.Domain
{
    public class Product : Entity<Guid>
    {
        public Product()
        {
            Id = SequentialGuidGenerator.GenerateId();
        }

        public string Name { get; set; }
    }
}
