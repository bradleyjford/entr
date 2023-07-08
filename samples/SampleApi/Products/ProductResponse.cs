namespace SampleApi.Products;

public class ProductResponse
{
    public ProductId Id { get; set; }
    public ProductName Name { get; set; } = default!;
}
