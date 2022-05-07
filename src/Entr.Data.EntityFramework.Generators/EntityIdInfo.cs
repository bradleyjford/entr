namespace Entr.Data.EntityFramework.Generators;

public sealed class EntityIdInfo
{
    public EntityIdInfo(string? @namespace, string name, string wrappedType)
    {
        Namespace = @namespace ?? string.Empty;
        Name = name;
        WrappedType = wrappedType;
    }

    public string Namespace { get; }
    public string Name { get; }
    public string WrappedType { get; }
}
