using Entr.CommandQuery;
using Entr.Domain;
using Microsoft.EntityFrameworkCore;

namespace Entr.Data.EntityFramework
{
    public class UnitOfWorkAsyncCommandHandlerDecorator<TCommand, TResponse> 
        : IAsyncCommandHandler<TCommand, TResponse>
        where TCommand : IAsyncCommand<TResponse>
    {
        readonly IAsyncCommandHandler<TCommand, TResponse> _decorated;
        readonly DbContext _dbContext;
        readonly IUserContext _userContext;

        protected UnitOfWorkAsyncCommandHandlerDecorator(
            IAsyncCommandHandler<TCommand, TResponse> decorated,
            DbContext dbContext,
            IUserContext userContext)
        {
            _decorated = decorated;
            _dbContext = dbContext;
            _userContext = userContext;
        }

        public async Task<TResponse> Handle(TCommand command)
        {
            var response = await _decorated.Handle(command);

            OnBeforeSaveChanges();

            DbContextInlineAuditor.ApplyInlineAuditValues(_dbContext, _userContext);

            await _dbContext.SaveChangesAsync();

            return response;
        }

        protected virtual void OnBeforeSaveChanges()
        {
        }
    }
}
