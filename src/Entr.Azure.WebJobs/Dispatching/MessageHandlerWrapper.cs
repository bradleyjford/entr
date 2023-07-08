using System;
using System.Threading;
using System.Threading.Tasks;

namespace Entr.Azure.WebJobs.Dispatching
{
    internal interface IMessageHandlerWrapper
    {
        Task Handle(object command, CancellationToken cancellationToken);
    }

    internal sealed class MessageHandlerWrapper<TRequest> : IMessageHandlerWrapper
    {
        private readonly IMessageHandler<TRequest> _decorated;

        public MessageHandlerWrapper(IMessageHandler<TRequest> decorated)
        {
            _decorated = decorated;
        }

        public Task Handle(object command, CancellationToken cancellationToken)
        {
            var typedCommand = (TRequest)command;

            return _decorated.Handle(typedCommand);
        }
    }
}
