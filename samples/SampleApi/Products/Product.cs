using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SampleApi.Products;

[EntityId<Guid>]
public partial struct ProductId { }

public interface ITest<out T, TValue>
{
    static abstract T Create(TValue value);

    public TValue Value { get; }
}

public class Test : ITest<Test, Guid>
{
    public static Test Create(Guid value)
    {
        return new Test(value);
    }

    public Test(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }
}

public sealed class IdValueConverter<TId, TValue> : ValueConverter<TId, TValue>
    where TId : ITest<TId, TValue>
{
    static readonly Func<ValueConverterInfo, ValueConverter> Factory =
        vci => new IdValueConverter<TId, TValue>(vci.MappingHints);

    public static readonly ValueConverterInfo DefaultInfo = new(
        modelClrType: typeof(TId),
        providerClrType: typeof(TValue),
        factory: Factory,
        null);

    public IdValueConverter() : this(null) { }
    public IdValueConverter(ConverterMappingHints mappingHints = null)
        : base(
            id => id.Value,
            value => Create(value),
            mappingHints
        )
    { }

    static TId Create(TValue value)
    {
        return TId.Create(value);
    }
}


public class TestFactory<TValue>
{
    public static TTest Blah<TTest>(TValue value)
        where TTest : ITest<TTest, TValue>
    {
        return TTest.Create(value);
    }
}

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
