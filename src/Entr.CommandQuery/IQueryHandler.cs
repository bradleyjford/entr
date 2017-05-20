namespace Entr.CommandQuery
{
    public interface IQueryHandler<TCommand, out TResult> : IRequestHandler<TCommand, TResult>
        where TCommand : IQuery<TResult>
    {
    }
}
