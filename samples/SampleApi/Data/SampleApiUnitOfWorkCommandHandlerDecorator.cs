using Microsoft.Extensions.Logging;
using SampleApi.Security;

namespace SampleApi.Data;

public sealed class SampleApiUnitOfWorkCommandHandlerDecorator<TCommand, TResponse>
    : UnitOfWorkAsyncCommandHandlerDecorator<TCommand, TResponse>
    where TCommand : IAsyncCommand<TResponse>
{
    readonly DbContext _dbContext;
    readonly UserContext _userContext;

    public SampleApiUnitOfWorkCommandHandlerDecorator(
        IAsyncCommandHandler<TCommand, TResponse> decorated,
        DbContext dbContext,
        UserContext userContext,
        ILogger<SampleApiUnitOfWorkCommandHandlerDecorator<TCommand, TResponse>> logger)
        : base(decorated, dbContext, logger)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    protected override async Task OnSavingChanges(TCommand command, TResponse result)
    {
        await DbContextInlineAuditor<User, UserId>.ApplyInlineAuditValues(_dbContext, _userContext);

    }
}
