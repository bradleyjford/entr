using System.ComponentModel;
using System.Text.Json.Serialization;

namespace SampleApi.Products;

[TypeConverter(typeof(ProductNameTypedStringTypeConverter))]
[JsonConverter(typeof(ProductNameJsonConverter))]
public class ProductName : TypedString<ProductName>
{
    public ProductName(string value)
        : base(value, 50, 5, StringComparison.OrdinalIgnoreCase)
    { }
}

public sealed class ProductNameJsonConverter : TypedStringJsonConverter<ProductName>
{
    public ProductNameJsonConverter()
        : base(value => new(value))
    { }
}

public sealed class ProductNameTypedStringTypeConverter : TypedStringTypeConverter<ProductName>
{
    public ProductNameTypedStringTypeConverter()
        : base(value => new(value))
    { }
}
