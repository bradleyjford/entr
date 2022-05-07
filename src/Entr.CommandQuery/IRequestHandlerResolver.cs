using System;

namespace Entr.CommandQuery;

public interface IRequestHandlerResolver
{
    object Resolve(Type type);
}
