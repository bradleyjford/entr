using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Entr.Domain;

public abstract class TypedInt32<T> : IEquatable<T>
    where T : TypedInt32<T>
{
    protected TypedInt32(int value, int minValue = int.MinValue, int maxValue = int.MaxValue)
    {
        if (value < minValue) throw new ArgumentOutOfRangeException(nameof(value));
        if (value > maxValue) throw new ArgumentOutOfRangeException(nameof(value));

        Value = value;
    }

    public int Value { get; }

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

public abstract class TypedInt32JsonConverter<T> : JsonConverter<T>
    where T : TypedInt32<T>
{
    readonly Func<int, T> _factory;

    protected TypedInt32JsonConverter(Func<int, T> factory)
    {
        _factory = factory;
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        _factory(reader.GetInt32()!);

    public override void Write(Utf8JsonWriter writer, T typedInt32, JsonSerializerOptions options)
        => writer.WriteNumberValue(typedInt32.Value);
}

public abstract class TypedInt32TypeConverter<T> : TypeConverter
    where T : TypedInt32<T>
{
    readonly Func<int, T> _factory;

    protected TypedInt32TypeConverter(Func<int, T> factory)
    {
        _factory = factory;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(int) || base.CanConvertFrom(context, sourceType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is int intValue)
        {
            return _factory(intValue);
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is T typedInt32 && destinationType == typeof(string))
        {
            return typedInt32.Value;
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
