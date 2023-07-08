namespace Entr.Domain;

public interface IUserContext<TUser, TUserId>
    where TUser : Entity<TUserId>
{
    Task<TUser?> GetCurrent();
    //Task<TUserId?> GetCurrentId();
}
