using System;

namespace Entr.Domain;

public interface IUserContext<out TUserId>
{
    TUserId UserId { get; }
}

public interface IUserContext
{
    object UserId { get; }
}

public abstract class UserContext<TUserId> : IUserContext<TUserId>, IUserContext
{
    public abstract TUserId UserId { get; }

    object IUserContext.UserId
    {
        get => UserId!;
    }
}
