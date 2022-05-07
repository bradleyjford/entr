using System.Collections.Generic;

namespace SampleApi.Security;

[EntityId<Guid>]
public partial struct UserId
{
    public static UserId Unknown = new(Guid.Empty);
}

public class User : Entity<UserId>
{
    public string Name { get; set; }
    public string Email { get; set; }

    public HashSet<Role> Roles { get; } = new();

    public User()
    {
        Id = UserId.New();
    }
}
