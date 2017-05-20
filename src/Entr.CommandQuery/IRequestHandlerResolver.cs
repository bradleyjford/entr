using System;

namespace Entr.CommandQuery
{
    public interface IRequestHandlerResolver : IDisposable
    {
        IRequestHandlerResolver CreateLifetimeScope();
        object Resolve(Type type);
    }
}
