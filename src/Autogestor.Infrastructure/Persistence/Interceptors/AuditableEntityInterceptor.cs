using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Autogestor.Infrastructure.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor(IUserContext userContext) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditEntities(context: eventData.Context!);
        return base.SavingChanges(eventData: eventData, result: result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditEntities(context: eventData.Context!);
        return base.SavingChangesAsync(
            eventData: eventData,
            result: result,
            cancellationToken: cancellationToken);
    }

    private void UpdateAuditEntities(DbContext context)
    {
        DateTime utcNow = DateTime.UtcNow;

        var addedEntries = context.ChangeTracker.Entries<AuditableEntity>()
            .Where(predicate: e => e.State == EntityState.Added)
            .ToList();

        if (addedEntries.Count > 0)
        {
            Guid currentUserId = userContext.UserId;
            foreach (EntityEntry<AuditableEntity> entry in addedEntries)
            {
                if (entry.Property(propertyExpression: e => e.CreatedAt).CurrentValue == default)
                    entry.Property(propertyExpression: e => e.CreatedAt).CurrentValue = utcNow;

                if (entry.Entity.CreatedBy == Guid.Empty)
                    entry.Property(propertyExpression: e => e.CreatedBy).CurrentValue = currentUserId;
            }
        }

        var modifiedEntries = context.ChangeTracker.Entries<AuditableEntity>()
            .Where(predicate: e => e.State == EntityState.Modified)
            .ToList();

        if (modifiedEntries.Count > 0)
        {
            Guid currentUserId = userContext.UserId;
            foreach (EntityEntry<AuditableEntity> entry in modifiedEntries)
            {
                entry.Property(propertyExpression: e => e.CreatedAt).IsModified = false;
                entry.Property(propertyExpression: e => e.CreatedBy).IsModified = false;
                entry.Property(propertyExpression: e => e.UpdatedAt).CurrentValue = utcNow;
                entry.Property(propertyExpression: e => e.UpdatedBy).CurrentValue = currentUserId;
            }
        }
    }
}
