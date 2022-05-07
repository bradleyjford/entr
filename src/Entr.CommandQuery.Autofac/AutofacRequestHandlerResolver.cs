using System;
using Autofac;

namespace Entr.CommandQuery.Autofac;

public class AutofacRequestHandlerResolver : IRequestHandlerResolver
{
    readonly ILifetimeScope _lifetimeScope;

    public AutofacRequestHandlerResolver(ILifetimeScope lifetimeScope)
    {
        _lifetimeScope = lifetimeScope;
    }

    public object Resolve(Type type)
    {
        try
        {
            return _lifetimeScope.Resolve(type);
        }
        catch (Exception ex)
        {
            throw new RequestHandlerNotFoundException(type, ex);
        }
    }
}
