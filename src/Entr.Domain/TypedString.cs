using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Entr.Domain;

public abstract class TypedString<T> : IEquatable<T>
    where T : TypedString<T>
{
    readonly StringComparison _stringComparison;

    protected TypedString(
        string value,
        int maximumLength,
        int minimumLength = 1,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        if (value.Length < minimumLength)
            throw new ArgumentException($"{nameof(T)} must be greater than {minimumLength} characters in length",
                nameof(value));

        if (value.Length > maximumLength)
            throw new ArgumentException($"{nameof(T)} must be less than {maximumLength} characters in length",
                nameof(value));

        _stringComparison = stringComparison;

        Value = value;
    }

    public string Value { get; }

    public bool Equals(T? other)
    {
        if (other is null) return false;

        return string.Compare(Value, other.Value, _stringComparison) == 0;
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

public abstract class TypedStringJsonConverter<T> : JsonConverter<T>
    where T : TypedString<T>
{
    readonly Func<string, T> _factory;

    protected TypedStringJsonConverter(Func<string, T> factory)
    {
        _factory = factory;
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        _factory(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, T name, JsonSerializerOptions options)
        => writer.WriteStringValue(name.Value);
}

public abstract class TypedStringTypeConverter<T> : TypeConverter
    where T : TypedString<T>
{
    readonly Func<string, T> _factory;

    protected TypedStringTypeConverter(Func<string, T> factory)
    {
        _factory = factory;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            return _factory(stringValue);
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is T stronglyTypedString && destinationType == typeof(string))
        {
            return stronglyTypedString.Value;
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
