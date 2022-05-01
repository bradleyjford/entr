using System;
using System.Linq;
using System.Threading.Tasks;
using Entr.CommandQuery;
using Entr.Domain;
using Microsoft.EntityFrameworkCore;

namespace Entr.Data.EntityFrameworkCore;

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
        RestoreRowVersions();

        await _dbContext.SaveChangesAsync();

        return response;
    }

    protected virtual void OnBeforeSaveChanges()
    {
    }

    void RestoreRowVersions()
    {
        foreach (var entry in _dbContext.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Deleted)
            {
                continue;
            }

            if (entry.CurrentValues.Properties.Any(p => p.Name == "RowVersion"))
            {
                var property = entry.Property("RowVersion");

                property.OriginalValue = property.CurrentValue;
                property.IsModified = false;
            }
        }
    }
}
