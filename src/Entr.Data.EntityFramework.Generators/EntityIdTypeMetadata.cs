namespace Entr.Data.EntityFramework.Generators;

public sealed class EntityIdTypeMetadata
{
    public EntityIdTypeMetadata(string containingNamespace, string name, string wrappedType)
    {
        ContainingNamespace = containingNamespace;
        Name = name;
        WrappedType = wrappedType;
    }

    public string ContainingNamespace { get; }
    public string Name { get; }
    public string WrappedType { get; }
}
