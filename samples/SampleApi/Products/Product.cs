using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SampleApi.Products;

[EntityId<Guid>]
public partial struct ProductId { }

public class Product : Entity<ProductId>
{
    public Product(ProductName name)
    {
        Id = ProductId.New();
        Name = name;
    }

    public ProductName Name { get; private set; }

    public void Rename(ProductName newName)
    {
        Name = newName;
    }
}
