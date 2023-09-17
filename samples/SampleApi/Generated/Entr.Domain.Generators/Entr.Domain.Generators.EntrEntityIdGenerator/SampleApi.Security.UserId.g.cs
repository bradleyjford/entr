﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the EntrEntityId source generator
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#pragma warning disable 1591 // publicly visible type or member must be documented

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Entr.Domain;

namespace SampleApi.Security;

[JsonConverter(typeof(UserIdJsonConverter))]
[TypeConverter(typeof(UserIdTypeConverter))]
readonly partial struct UserId : IId<UserId, System.Guid>, IEquatable<UserId>
{
    public static UserId New()
        => new UserId(SequentialGuidGenerator.Generate());

    public UserId(System.Guid value)
    {
        if (value == default)
        {
            throw new ArgumentException("Specified value cannot be default(System.Guid)", nameof(value));
        }

        Value = value;
    }

    public System.Guid Value { get; }

    public override int GetHashCode()
    {
        return HashCode.Combine(typeof(UserId), Value);
    }

    public override string ToString() => Value.ToString();

    public override bool Equals(object obj)
        => obj is UserId other && Equals(this, other);
    public bool Equals(UserId other)
        => Equals(this, other);

    public static bool Equals(UserId? left, UserId? right)
    {
        if (!left.HasValue && !right.HasValue) return true;
        if (!left.HasValue || !right.HasValue) return false;

        return left.Value.Value == right.Value.Value;
    }

    public static bool operator ==(UserId left, UserId right) =>
        Equals(left, right);
    public static bool operator !=(UserId left, UserId right) =>
        !Equals(left, right);

    public sealed class UserIdJsonConverter : JsonConverter<UserId>
    {
        public override UserId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
           => new UserId(reader.GetGuid()!);

        public override void Write(Utf8JsonWriter writer, UserId id, JsonSerializerOptions options)
           => writer.WriteStringValue(id.Value);
      }

    public sealed class UserIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(System.Guid) || sourceType == typeof(string);
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(System.Guid) || destinationType == typeof(string);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                return new UserId(System.Guid.Parse((string)value));
            }

            return new UserId((System.Guid)value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((UserId)value).Value.ToString();
            }

            return ((UserId)value).Value;
        }
    }
}