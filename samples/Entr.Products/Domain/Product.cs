using System;
using Entr.Domain;

namespace Entr.Products.Domain
{
    [EntrEntityId<Guid>]
    public class ProductId { }

    public class Product : Entity<ProductId>
    {
        public Product()
        {
            Id = ProductId.New();
        }

        public string Name { get; set; }
    }
}
