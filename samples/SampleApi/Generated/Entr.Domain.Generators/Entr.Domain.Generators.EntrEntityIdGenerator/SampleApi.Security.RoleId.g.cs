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

[JsonConverter(typeof(RoleIdJsonConverter))]
[TypeConverter(typeof(RoleIdTypeConverter))]
readonly partial struct RoleId : IEquatable<RoleId>
{
    public static RoleId New()
        => new RoleId(SequentialGuidGenerator.Generate());

    public RoleId(System.Guid value)
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
        return HashCode.Combine(typeof(RoleId), Value);
    }

    public override string ToString() => Value.ToString();

    public override bool Equals(object obj)
        => obj is RoleId other && Equals(this, other);
    public bool Equals(RoleId other)
        => Equals(this, other);

    public static bool Equals(RoleId? left, RoleId? right)
    {
        if (!left.HasValue && !right.HasValue) return true;
        if (!left.HasValue || !right.HasValue) return false;

        return left.Value.Value == right.Value.Value;
    }

    public static bool operator ==(RoleId left, RoleId right) =>
        Equals(left, right);
    public static bool operator !=(RoleId left, RoleId right) =>
        !Equals(left, right);

    public sealed class RoleIdJsonConverter : JsonConverter<RoleId>
    {
        public override RoleId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
           => new RoleId(reader.GetGuid()!);

        public override void Write(Utf8JsonWriter writer, RoleId id, JsonSerializerOptions options)
           => writer.WriteStringValue(id.Value);
      }

    public sealed class RoleIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(System.Guid) || sourceType == typeof(string);
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(System.Guid) || destinationType == typeof(string);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                return new RoleId(System.Guid.Parse((string)value));
            }

            return new RoleId((System.Guid)value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((RoleId)value).Value.ToString();
            }

            return ((RoleId)value).Value;
        }
    }
}
