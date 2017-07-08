using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Entr.CommandQuery
{
    public interface IMediator
    {
        TResponse Send<TResponse>(ICommand<TResponse> command);
        TResponse Send<TResponse>(IQuery<TResponse> query);

        Task<TResponse> SendAsync<TResponse>(IAsyncQuery<TResponse> command);
        Task<TResponse> SendAsync<TResponse>(IAsyncCommand<TResponse> query);
    }

    public class Mediator : IMediator
    {
        static readonly Type AsyncRequestHandlerWrapperType = typeof(AsyncRequestHandlerWrapper<,>);
        static readonly Type RequestHandlerWrapperType = typeof(RequestHandlerWrapper<,>);

        static readonly Type AsyncCommandHandlerInterfaceType = typeof(IAsyncCommandHandler<,>);
        static readonly Type CommandHandlerInterfaceType = typeof(ICommandHandler<,>);

        static readonly Type AsyncQueryHandlerInterfaceType = typeof(IAsyncQueryHandler<,>);
        static readonly Type QueryHandlerInterfaceType = typeof(IQueryHandler<,>);

        readonly IRequestHandlerResolver _requestHandlerResolver;

        readonly ConcurrentDictionary<Type, Type> _handlerTypes = new ConcurrentDictionary<Type, Type>();
        readonly ConcurrentDictionary<Type, Type> _wrappedHandlerTypes = new ConcurrentDictionary<Type, Type>();

        public Mediator(IRequestHandlerResolver requestHandlerResolver)
        {
            _requestHandlerResolver = requestHandlerResolver;
        }

        public TResponse Send<TResponse>(ICommand<TResponse> command)
        {
            var handler = ResolveRequestHandler<TResponse>(
                CommandHandlerInterfaceType,
                command.GetType());

            return handler.Handle(command);
        }

        public TResponse Send<TResponse>(IQuery<TResponse> query)
        {
            var handler = ResolveRequestHandler<TResponse>(
                CommandHandlerInterfaceType,
                query.GetType());

            return handler.Handle(query);
        }

        IRequestHandlerWrapper<TResponse> ResolveRequestHandler<TResponse>(
            Type requestHandlerInterfaceType,
            Type requestType)
        {
            var responseType = typeof(TResponse);

            var requestHandlerType = _handlerTypes.GetOrAdd(requestType, r => requestHandlerInterfaceType.MakeGenericType(r, responseType));
            var wrappedHandlerType = _wrappedHandlerTypes.GetOrAdd(requestType, r => RequestHandlerWrapperType.MakeGenericType(r, responseType));

            var handler = _requestHandlerResolver.Resolve(requestHandlerType);

            return (IRequestHandlerWrapper<TResponse>)Activator.CreateInstance(wrappedHandlerType, handler);
        }

        public Task<TResponse> SendAsync<TResponse>(IAsyncCommand<TResponse> command)
        {
            var handler = ResolveAsyncHandler<TResponse>(
                AsyncCommandHandlerInterfaceType,
                command.GetType());

            return handler.Handle(command);
        }

        public Task<TResponse> SendAsync<TResponse>(IAsyncQuery<TResponse> query)
        {
            var handler = ResolveAsyncHandler<TResponse>(
                AsyncQueryHandlerInterfaceType,
                query.GetType());

            return handler.Handle(query);
        }

        IAsyncRequestHandlerWrapper<TResponse> ResolveAsyncHandler<TResponse>(
            Type requestHandlerInterfaceType, 
            Type requestType)
        {
            var responseType = typeof(TResponse);

            var requestHandlerType = _handlerTypes.GetOrAdd(requestType, r => requestHandlerInterfaceType.MakeGenericType(r, responseType));
            var wrappedHandlerType = _wrappedHandlerTypes.GetOrAdd(requestType, r => AsyncRequestHandlerWrapperType.MakeGenericType(r, responseType));

            var handler = _requestHandlerResolver.Resolve(requestHandlerType);

            return (IAsyncRequestHandlerWrapper<TResponse>)Activator.CreateInstance(wrappedHandlerType, handler);
        }
    }
}
