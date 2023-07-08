using System.Collections.Generic;

namespace SampleApi.Security;

[EntityId<Guid>]
public partial struct UserId
{
    public static readonly UserId Unknown = new(Guid.Empty);
}

public class User : Entity<UserId>
{
    public string Name { get; }
    public EmailAddress Email { get; }

    public HashSet<Role> Roles { get; } = new();

    protected User() { }

    public User(string name, EmailAddress email)
    {
        Id = UserId.New();

        Name = name;
        Email = email;
    }
}
