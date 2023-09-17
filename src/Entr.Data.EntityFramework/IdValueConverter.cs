using Entr.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Entr.Data.EntityFramework;

public class IdValueConverter<TId, TValue> : ValueConverter<TId, TValue>
    where TId : IId<TId, TValue>
{
    static readonly Func<ValueConverterInfo, ValueConverter> Factory =
        vci => new IdValueConverter<TId, TValue>(vci.MappingHints);

    public static readonly ValueConverterInfo DefaultInfo = new(
        modelClrType: typeof(TId),
        providerClrType: typeof(TValue),
        factory: Factory,
        null);

    public IdValueConverter() : this(null) { }
    public IdValueConverter(ConverterMappingHints? mappingHints = null)
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
