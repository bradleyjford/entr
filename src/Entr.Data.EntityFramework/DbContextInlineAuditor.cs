using Entr.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Entr.Data.EntityFramework
{
    public static class DbContextInlineAuditor<TUser, TUserId>
        where TUser : Entity<TUserId>
    {
        const string CreatedUtcDatePropertyName = nameof(IInlineAuditedEntity.CreatedUtcDate);
        const string CreatedByUserIdPropertyName = nameof(IInlineAuditedEntity.CreatedByUserId);

        const string ModifiedUtcDatePropertyName = nameof(IInlineAuditedEntity.ModifiedUtcDate);
        const string ModifiedByUserIdPropertyName = nameof(IInlineAuditedEntity.ModifiedByUserId);

        public static async Task ApplyInlineAuditValues(
            DbContext dbContext,
            IUserContext<TUser, TUserId> userContext)
        {
            foreach (var entry in dbContext.ChangeTracker.Entries())
            {
                if (entry.Entity is not IInlineAuditedEntity)
                {
                    continue;
                }

                var user = await userContext.GetCurrent();

                if (user is null)
                {
                    return;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        SetCreated(user.Id, entry);
                        SetModified(user.Id, entry);
                        break;
                    case EntityState.Modified:
                        SetModified(user.Id, entry);
                        break;
                }
            }
        }

        static void SetCreated(TUserId userId, EntityEntry entry)
        {
            var existingCreatedUtcDate = entry.CurrentValues[CreatedUtcDatePropertyName] as DateTime?;

            // Do not attempt to set the CreatedUtcDate if a value has already been supplied.
            if (existingCreatedUtcDate == null || existingCreatedUtcDate == default(DateTime))
            {
                entry.CurrentValues[CreatedUtcDatePropertyName] = ClockProvider.GetUtcNow();
            }

            entry.CurrentValues[CreatedByUserIdPropertyName] = userId;
        }

        static void SetModified(TUserId userId, EntityEntry entry)
        {
            if (entry.OriginalValues[ModifiedUtcDatePropertyName] == entry.CurrentValues[ModifiedUtcDatePropertyName])
            {
                entry.CurrentValues[ModifiedUtcDatePropertyName] = ClockProvider.GetUtcNow();
            }

            entry.CurrentValues[ModifiedByUserIdPropertyName] = userId;
        }
    }
}
