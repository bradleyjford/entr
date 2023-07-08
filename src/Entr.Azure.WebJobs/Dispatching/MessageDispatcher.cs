using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Entr.Azure.WebJobs.Dispatching
{
    public interface IMessageDispatcher
    {
        Task Send(object request, CancellationToken cancellationToken = default);
    }

    [DebuggerNonUserCode]
    public sealed class MessageDispatcher : IMessageDispatcher
    {
        private static readonly ConcurrentDictionary<Type, Type> _requestHandlerTypes = 
            new ConcurrentDictionary<Type, Type>();

        private static readonly ConcurrentDictionary<Type, Type> _messageHandlerWrapperTypes =
            new ConcurrentDictionary<Type, Type>();

        private readonly IServiceProvider _serviceProvider;

        public MessageDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [DebuggerNonUserCode]
        public Task Send(object request, CancellationToken cancellationToken = default)
        {
            var requestType = request.GetType();

            var handler = ResolveRequestHandler(requestType);

            return handler.Handle(request, cancellationToken);
        }

        private IMessageHandlerWrapper ResolveRequestHandler(Type requestType)
        {
            var requestHandlerType = _requestHandlerTypes.GetOrAdd(
                requestType, 
                rt => typeof(IMessageHandler<>).MakeGenericType(rt));
                
            var wrapperType = _messageHandlerWrapperTypes.GetOrAdd(
                requestType, 
                rt => typeof(MessageHandlerWrapper<>).MakeGenericType(rt));

            var handler = GetHandler(requestHandlerType);

            return (IMessageHandlerWrapper)Activator.CreateInstance(wrapperType, handler);
        }

        private object GetHandler(Type handlerType)
        {
            var handler = _serviceProvider.GetService(handlerType);
            
            if (handler == null)
            { 
                throw new HandlerNotFoundException(handlerType);
            }

            return handler;
        }
    }
}
