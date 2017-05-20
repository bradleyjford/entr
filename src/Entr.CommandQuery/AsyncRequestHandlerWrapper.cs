using System;
using System.Threading.Tasks;

namespace Entr.CommandQuery
{
    interface IAsyncRequestHandlerWrapper<TResponse>
    {
        Task<TResponse> Handle(IAsyncRequest<TResponse> request);
    }

    class AsyncRequestHandlerWrapper<TRequest, TResponse> : IAsyncRequestHandlerWrapper<TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        readonly IAsyncRequestHandler<TRequest, TResponse> _decorated;

        public AsyncRequestHandlerWrapper(IAsyncRequestHandler<TRequest, TResponse> decorated)
        {
            _decorated = decorated;
        }

        public Task<TResponse> Handle(IAsyncRequest<TResponse> request)
        {
            var typedCommand = (TRequest)request;

            return _decorated.Handle(typedCommand);
        }
    }
}
