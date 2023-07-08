using JetBrains.Annotations;

namespace Entr.CommandQuery;

public interface IAsyncRequest<TRequest>
{
}

public interface IAsyncRequestHandler<in TRequest, TResponse>
    where TRequest : IAsyncRequest<TResponse>
{
    Task<TResponse> Handle(TRequest command);
}

public interface IAsyncCommand<TResult> : IAsyncRequest<TResult>
{
}

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IAsyncCommandHandler<in TCommand, TResult> : IAsyncRequestHandler<TCommand, TResult>
    where TCommand : IAsyncCommand<TResult>
{
}

public interface IAsyncQuery<TResult> : IAsyncRequest<TResult>
{
}

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IAsyncQueryHandler<in TQuery, TResult> : IAsyncRequestHandler<TQuery, TResult>
    where TQuery : IAsyncQuery<TResult>
{
}
