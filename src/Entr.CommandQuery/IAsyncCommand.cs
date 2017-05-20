namespace Entr.CommandQuery
{
    public interface IAsyncCommand<out TResult> : IAsyncRequest<TResult>
    {
    }
}
