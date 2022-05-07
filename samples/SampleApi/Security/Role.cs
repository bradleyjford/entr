namespace SampleApi.Security;

[EntityId<Guid>]
public partial struct RoleId
{
}

public class Role : Entity<RoleId>
{
    protected Role()
    {
    }

    public Role(string name)
    {
        Name = name;
    }

    public string Name { get; protected set; } = default!;
}
