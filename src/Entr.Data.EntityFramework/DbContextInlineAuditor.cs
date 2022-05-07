using Entr.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Entr.Data.EntityFramework
{
    public static class DbContextInlineAuditor
    {
        const string CreatedUtcDatePropertyName = nameof(IInlineAuditedEntity.CreatedUtcDate);
        const string CreatedByUserIdPropertyName = nameof(IInlineAuditedEntity.CreatedByUserId);

        const string ModifiedUtcDatePropertyName = nameof(IInlineAuditedEntity.ModifiedUtcDate);
        const string ModifiedByUserIdPropertyName = nameof(IInlineAuditedEntity.ModifiedByUserId);
        
        public static void ApplyInlineAuditValues(DbContext dbContext, IUserContext userContext)
        {
            foreach (var entry in dbContext.ChangeTracker.Entries())
            {
                if (entry.Entity is not IInlineAuditedEntity)
                {
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        SetCreated(userContext, entry);
                        SetModified(userContext, entry);
                        break;
                    case EntityState.Modified:
                        SetModified(userContext, entry);
                        break;
                }
            }
        }

        static void SetCreated(IUserContext userContext, EntityEntry entry)
        {
            var existingCreatedUtcDate = entry.CurrentValues[CreatedUtcDatePropertyName] as DateTime?;

            // Do not attempt to set the CreatedUtcDate if a value has already been supplied.
            if (existingCreatedUtcDate == null || existingCreatedUtcDate == default(DateTime))
            {
                entry.CurrentValues[CreatedUtcDatePropertyName] = ClockProvider.GetUtcNow();
            }

            entry.CurrentValues[CreatedByUserIdPropertyName] = userContext.UserId;
        }

        static void SetModified(IUserContext userContext, EntityEntry entry)
        {
            if (entry.OriginalValues[ModifiedUtcDatePropertyName] == entry.CurrentValues[ModifiedUtcDatePropertyName])
            {
                entry.CurrentValues[ModifiedUtcDatePropertyName] = ClockProvider.GetUtcNow();
            }

            entry.CurrentValues[ModifiedByUserIdPropertyName] = userContext.UserId;
        }
    }
}
