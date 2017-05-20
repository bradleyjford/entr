namespace Entr.CommandQuery
{
    public interface IAsyncQueryHandler<TQuery, TResult> : IAsyncRequestHandler<TQuery, TResult>
        where TQuery : IAsyncQuery<TResult>
    {
    }
}
