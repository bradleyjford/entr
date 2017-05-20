namespace Entr.CommandQuery
{
    public interface IAsyncCommandHandler<TCommand, TResult> : IAsyncRequestHandler<TCommand, TResult>
        where TCommand : IAsyncCommand<TResult>
    {
    }
}
