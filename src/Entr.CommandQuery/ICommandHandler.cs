namespace Entr.CommandQuery
{
    public interface ICommandHandler<TCommand, out TResult> : IRequestHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
    }
}
