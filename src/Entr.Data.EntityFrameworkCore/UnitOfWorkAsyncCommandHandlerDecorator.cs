using System;
using System.Linq;
using System.Threading.Tasks;
using Entr.CommandQuery;
using Entr.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Entr.Data
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

            ApplyInlineAuditValues();
            RestoreRowVersions();

            await _dbContext.SaveChangesAsync();

            return response;
        }

        protected virtual void OnBeforeSaveChanges()
        {
        }

        void ApplyInlineAuditValues()
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                if (entry.Entity is IInlineAuditedEntity<Guid>)
                {
                    if (entry.State == EntityState.Added)
                    {
                        SetCreated(entry);
                        SetModified(entry);
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        SetModified(entry);
                    }
                }
            }
        }

        void SetCreated(EntityEntry entry)
        {
            entry.CurrentValues["CreatedUtcDate"] = ClockProvider.GetUtcNow();
            entry.CurrentValues["CreatedByUserId"] = _userContext.UserId;
        }

        void SetModified(EntityEntry entry)
        {
            entry.CurrentValues["ModifiedUtcDate"] = ClockProvider.GetUtcNow();
            entry.CurrentValues["ModifiedByUserId"] = _userContext.UserId;
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
}
