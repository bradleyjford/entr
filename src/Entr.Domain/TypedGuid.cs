using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Entr.Domain;

public abstract class TypedGuid<T> : IEquatable<T>
    where T : TypedGuid<T>
{
    protected TypedGuid(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public bool Equals(T? other)
    {
        if (other is null) return false;

        return Value.Equals(other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;

        if (obj.GetType() != this.GetType()) return false;

        return Equals((T)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(typeof(T), Value);
    }
}

public abstract class TypedGuidJsonConverter<T> : JsonConverter<T>
    where T : TypedGuid<T>
{
    readonly Func<Guid, T> _factory;

    protected TypedGuidJsonConverter(Func<Guid, T> factory)
    {
        _factory = factory;
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        _factory(reader.GetGuid()!);

    public override void Write(Utf8JsonWriter writer, T name, JsonSerializerOptions options)
        => writer.WriteStringValue(name.Value);
}

public abstract class TypedGuidTypeConverter<T> : TypeConverter
    where T : TypedGuid<T>
{
    readonly Func<Guid, T> _factory;

    protected TypedGuidTypeConverter(Func<Guid, T> factory)
    {
        _factory = factory;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is Guid guidValue)
        {
            return _factory(guidValue);
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is T typedGuid && destinationType == typeof(string))
        {
            return typedGuid.Value;
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
