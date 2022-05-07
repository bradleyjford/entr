namespace SampleApi.Products;

[EntityId<Guid>]
public partial struct ProductId
{
}

public class Product : Entity<ProductId>
{
    protected Product()
    {
    }

    public Product(string name)
    {
        Id = ProductId.New();
        Name = name;
    }
    
    public string Name { get; set; }
}
