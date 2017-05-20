namespace Entr.CommandQuery
{
    public interface ICommand<out TResult> : IRequest<TResult>
    {
    }
}
