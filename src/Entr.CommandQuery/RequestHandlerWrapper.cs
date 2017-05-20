namespace Entr.CommandQuery
{
    interface IRequestHandlerWrapper<TResponse>
    {
        TResponse Handle(IRequest<TResponse> request);
    }

    class RequestHandlerWrapper<TRequest, TResponse> : IRequestHandlerWrapper<TResponse>
        where TRequest : IRequest<TResponse>
    {
        readonly IRequestHandler<TRequest, TResponse> _decorated;

        public RequestHandlerWrapper(IRequestHandler<TRequest, TResponse> decorated)
        {
            _decorated = decorated;
        }

        public TResponse Handle(IRequest<TResponse> request)
        {
            var typedCommand = (TRequest) request;

            return _decorated.Handle(typedCommand);
        }
    }
}
