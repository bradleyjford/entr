namespace SampleApi.Security;

public class AppUserContext : UserContext<UserId>
{
    public override UserId UserId => UserId.Unknown;
}
