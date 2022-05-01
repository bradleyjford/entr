using Microsoft.CodeAnalysis;

namespace Entr.Domain.SourceGenerators;

public class EntityIdToGenerate
{
    public EntityIdToGenerate(string containingNamespace, string fullName, string valueType)
    {
        ContainingNamespace = containingNamespace;
        FullName = fullName;
        ValueType = valueType;
    }

    public string ContainingNamespace { get; }
    public string FullName { get; }
    public string ValueType { get; }
}
