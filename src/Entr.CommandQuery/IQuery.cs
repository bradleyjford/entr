namespace Entr.CommandQuery
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}
