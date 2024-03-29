﻿using System.Collections.Immutable;
using System.Text;

namespace Entr.Domain.Generators;

internal static class SourceCodeGenerator
{
    internal const string Attribute = @"
namespace Entr.Domain
{
    [System.AttributeUsage(System.AttributeTargets.Struct)]
    public class EntityIdAttribute<TValue> : System.Attribute
    {
    }
}
";

    private const string Header = @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the EntrEntityId source generator
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#pragma warning disable 1591 // publicly visible type or member must be documented
";

    public static string GenerateSource(ImmutableArray<EntityIdInfo> entityIds)
    {
        var builder = new StringBuilder();

        builder.Append(Header);

        foreach (var entityIdClass in entityIds)
        {
            GenerateEntityIdType(entityIdClass, builder);
        }
        
        return builder.ToString();
    }

    internal static void GenerateEntityIdType(EntityIdInfo idInfo, StringBuilder builder)
    {
        builder.Append(@$"
namespace {idInfo.Namespace}
{{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Entr.Domain;

    [JsonConverter(typeof({idInfo.Name}JsonConverter))]
    [TypeConverter(typeof({idInfo.Name}TypeConverter))]
    readonly partial struct {idInfo.Name} : IEquatable<{idInfo.Name}>
    {{");

        if (idInfo.WrappedType is "Guid" or "System.Guid")
        {
            builder.Append(@$"
        public static {idInfo.Name} New()
            => new {idInfo.Name}(SequentialGuidGenerator.Generate());
");
        }

        builder.Append(@$"
        public {idInfo.Name}({idInfo.WrappedType} value)
        {{
            if (value == default)
            {{
                throw new ArgumentException(""Specified value cannot be default({idInfo.WrappedType})"", nameof(value));
            }}

            Value = value;
        }}

        public {idInfo.WrappedType} Value {{ get; }}

        public override int GetHashCode()
        {{
            var hashCode = HashCodeUtility.Seed;
            hashCode = HashCodeUtility.Hash(hashCode, typeof({idInfo.Name}));
            hashCode = HashCodeUtility.Hash(hashCode, Value);
            return hashCode;
        }}

        public override string ToString() => Value.ToString();

        public override bool Equals(object obj)
            => obj is {idInfo.Name} other && Equals(this, other);
        public bool Equals({idInfo.Name} other)
            => Equals(this, other);

        public static bool Equals({idInfo.Name}? left, {idInfo.Name}? right)
        {{
            if (!left.HasValue && !right.HasValue) return true;
            if (!left.HasValue || !right.HasValue) return false;
                
            return left.Value.Value == right.Value.Value;
        }}
        
        public static bool operator ==({idInfo.Name} left, {idInfo.Name} right) => 
            Equals(left, right);
        public static bool operator !=({idInfo.Name} left, {idInfo.Name} right) => 
            !Equals(left, right);

        public sealed class {idInfo.Name}JsonConverter : JsonConverter<{idInfo.Name}>
        {{
            public override {idInfo.Name} Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    ");
            if (idInfo.WrappedType == "System.Guid" || idInfo.WrappedType == "Guid")
            {
                builder.AppendLine($"           => new {idInfo.Name}(reader.GetGuid()!);");
            }
            else if (idInfo.WrappedType == "System.Int32" || idInfo.WrappedType == "int")
            {
                builder.AppendLine($"           => new {idInfo.Name}(reader.GetInt32()!);");
            }
            
            builder.Append(@$"
            public override void Write(Utf8JsonWriter writer, {idInfo.Name} id, JsonSerializerOptions options) 
    ");
            if (idInfo.WrappedType == "System.Guid" || idInfo.WrappedType == "Guid")
            {
                builder.AppendLine($"           => writer.WriteStringValue(id.Value);");
            }
            else if (idInfo.WrappedType == "System.Int32" || idInfo.WrappedType == "int")
            {
                builder.AppendLine($"           => writer.WriteNumberValue(id.Value);");
            }

            builder.Append(@$"      }}

        public sealed class {idInfo.Name}TypeConverter : TypeConverter
        {{
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
                => sourceType == typeof({idInfo.WrappedType}) || sourceType == typeof(string);
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
                => destinationType == typeof({idInfo.WrappedType}) || destinationType == typeof(string);

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {{
                if (value.GetType() == typeof(string))
                {{
                    return new {idInfo.Name}({idInfo.WrappedType}.Parse((string)value));
                }}

                return new {idInfo.Name}(({idInfo.WrappedType})value);
            }}
        
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {{
                if (destinationType == typeof(string))
                {{
                    return (({idInfo.Name})value).Value.ToString();
                }}

                return (({idInfo.Name})value).Value;
            }}
        }}
    }}
}}
");
    }
}
