using System;
using System.Text;

namespace Entr.Domain.SourceGenerators;

internal class SourceCodeGenerator
{
    internal const string Attribute = @"
namespace Entr.Domain
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class EntrEntityIdAttribute<TValue> : System.Attribute
    {
    }
}";

    private const string Header = @"
";

    public static string GenerateEntityIdClass(List<EntityIdToGenerate> entityIdClasses)
    {
        var builder = new StringBuilder();

        builder.Append(Header);

        foreach (var entityIdClass in entityIdClasses)
        {
            GenerateClassForEntityId(entityIdClass, builder);
        }

        return builder.ToString();
    }

    private static void GenerateClassForEntityId(EntityIdToGenerate c, StringBuilder builder)
    {
        builder.Append(@$"
namespace {c.ContainingNamespace}
{{
    using System;
    using Entr.Domain;

    public class {c.FullName}
    {{
        private readonly {c.ValueType} _value;
        private int _hashCode = 0;

");

        if (c.ValueType == "Guid")
        {
            builder.Append(@$"
        public static {c.FullName} New()
        {{
            return new {c.FullName}(SequentialGuidGenerator.GenerateId());
        }}
");
        }

        builder.Append(@$"
        public {c.FullName}({c.ValueType} value)
        {{
            if (value == default)
            {{
                throw new ArgumentException(""Specified value cannot be Guid.Empty"", nameof(value));
            }}

            _value = value;
        }}

        protected {c.FullName}()
        {{
        }}

        public override bool Equals(object other)
        {{
            var other = obj as {c.FullName};

            if (other is null)
            {{
                return false;
            }}

            return _value.Equals(other._value);
        }}

        public override int GetHashCode()
        {{
            if (_hashCode == 0)
            {{
                _hashCode = CalculateHashCode();
            }}

            return _hashCode;
        }}

        private int CalculateHashCode()
        {{
            var hashCode = HashCodeUtility.Seed;
            hashCode = HashCodeUtility.Hash(hashCode, GetType());
            hashCode = HashCodeUtility.Hash(hashCode, _value);
            return hashCode;
        }}

        public override string ToString()
        {{
            return $""<{c.FullName}>{{_value}}"";
        }}

        public static bool operator ==({c.FullName} left, {c.FullName} right)
        {{
            return Equals(left, right);
        }}

        public static bool operator !=({c.FullName} left, {c.FullName} right)
        {{
            return !Equals(left, right);
        }}
    }}
}}

");
    }
}
