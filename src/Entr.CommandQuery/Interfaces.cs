using System.Threading.Tasks;

namespace Entr.CommandQuery
{
    public interface IRequest<out TResponse>
    {
    }

    public interface IRequestHandler<in TRequest, out TResponse>
    {
        TResponse Handle(TRequest request);
    }

    public interface IAsyncRequest<TRequest>
    {
    }

    public interface IAsyncRequestHandler<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request);
    }

    public interface ICommand<out TResult> : IRequest<TResult>
    {
    }

    public interface ICommandHandler<in TCommand, out TResult> : IRequestHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
    }

    public interface IAsyncCommand<TResult> : IAsyncRequest<TResult>
    {
    }

    public interface IAsyncCommandHandler<TCommand, TResult> : IAsyncRequestHandler<TCommand, TResult>
        where TCommand : IAsyncCommand<TResult>
    {
    }

    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }

    public interface IQueryHandler<in TQuery, out TResult> : IRequestHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
    }

    public interface IAsyncQuery<TResult> : IAsyncRequest<TResult>
    {
    }

    public interface IAsyncQueryHandler<TQuery, TResult> : IAsyncRequestHandler<TQuery, TResult>
        where TQuery : IAsyncQuery<TResult>
    {
    }
}
