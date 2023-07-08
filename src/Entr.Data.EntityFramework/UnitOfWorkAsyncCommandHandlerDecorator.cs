using Entr.CommandQuery;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Entr.Data.EntityFramework
{
    public class UnitOfWorkAsyncCommandHandlerDecorator<TCommand, TResponse>
        : IAsyncCommandHandler<TCommand, TResponse>
        where TCommand : IAsyncCommand<TResponse>
    {
        readonly IAsyncCommandHandler<TCommand, TResponse> _decorated;
        readonly DbContext _dbContext;
        readonly ILogger _logger;

        protected UnitOfWorkAsyncCommandHandlerDecorator(
            IAsyncCommandHandler<TCommand, TResponse> decorated,
            DbContext dbContext,
            ILogger logger)
        {
            _decorated = decorated;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TCommand command)
        {
            await OnStarting(command);

            var response = await _decorated.Handle(command);

            await OnSavingChanges(command, response);

            var recordsAffected = await _dbContext.SaveChangesAsync();

            await OnCommitted(command, response, recordsAffected);

            return response;
        }

        protected virtual Task OnStarting(TCommand command)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnSavingChanges(TCommand command, TResponse response)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnCommitted(TCommand command, TResponse response, int recordsAffected)
        {
            return Task.CompletedTask;
        }
    }
}
