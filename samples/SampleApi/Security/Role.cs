namespace SampleApi.Security;

[EntityId<Guid>]
public partial struct RoleId
{
}

public class Role : Entity<RoleId>
{
    public string Name { get; protected set; }
}
