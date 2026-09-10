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
        Guid? currentUserId = null;

        foreach (EntityEntry<AuditableEntity> entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                currentUserId ??= userContext.UserId;
                if (entry.Property(propertyExpression: e => e.CreatedAt).CurrentValue == default)
                    entry.Property(propertyExpression: e => e.CreatedAt).CurrentValue = utcNow;

                if (entry.Entity.CreatedBy == Guid.Empty)
                    entry.Property(propertyExpression: e => e.CreatedBy).CurrentValue = currentUserId.Value;
            }
            else if (entry.State == EntityState.Modified)
            {
                currentUserId ??= userContext.UserId;
                entry.Property(propertyExpression: e => e.CreatedAt).IsModified = false;
                entry.Property(propertyExpression: e => e.CreatedBy).IsModified = false;
                entry.Property(propertyExpression: e => e.UpdatedAt).CurrentValue = utcNow;
                entry.Property(propertyExpression: e => e.UpdatedBy).CurrentValue = currentUserId.Value;
            }
        }
    }
}
