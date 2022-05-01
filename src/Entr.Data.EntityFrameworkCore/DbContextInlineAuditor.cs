using System;
using Entr.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Entr.Data.EntityFrameworkCore;

public static class DbContextInlineAuditor
{
    public static void ApplyInlineAuditValues(DbContext dbContext, IUserContext userContext)
    {
        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            var inlineAudited = entry.Entity as IInlineAuditedEntity;

            if (inlineAudited == null)
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
        var existingCreatedUtcDate = entry.CurrentValues["CreatedUtcDate"] as DateTime?;

        // Do not attempt to set the CreatedUtcDate if a value has already been supplied.
        if (existingCreatedUtcDate == null || existingCreatedUtcDate == default(DateTime))
        {
            entry.CurrentValues["CreatedUtcDate"] = ClockProvider.GetUtcNow();
        }

        entry.CurrentValues["CreatedByUserId"] = userContext.UserId;
    }

    static void SetModified(IUserContext userContext, EntityEntry entry)
    {
        entry.CurrentValues["ModifiedUtcDate"] = ClockProvider.GetUtcNow();
        entry.CurrentValues["ModifiedByUserId"] = userContext.UserId;
    }
}
