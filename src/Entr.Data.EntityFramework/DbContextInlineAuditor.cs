using Entr.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Entr.Data.EntityFramework
{
    public static class DbContextInlineAuditor
    {
        private const string CreatedUtcDatePropertyName = "CreatedUtcDate";
        private const string CreatedByUserIdPropertyName = "CreatedByUserId";

        private const string ModifiedUtcDatePropertyName = "ModifiedUtcDate";
        private const string ModifiedByUserIdPropertyName = "ModifiedByUserId";
        
        public static void ApplyInlineAuditValues(DbContext dbContext, IUserContext userContext)
        {
            foreach (var entry in dbContext.ChangeTracker.Entries())
            {
                if (!(entry.Entity is IInlineAuditedEntity))
                {
                    continue;
                }

                if (entry.State == EntityState.Added)
                {
                    SetCreated(userContext, entry);
                    SetModified(userContext, entry);
                }
                else if (entry.State == EntityState.Modified)
                {
                    SetModified(userContext, entry);
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
