using Autogestor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Autogestor.Infrastructure.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditEntities(context: eventData.Context);
        return base.SavingChanges(eventData: eventData, result: result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditEntities(context: eventData.Context);
        return base.SavingChangesAsync(
            eventData: eventData,
            result: result,
            cancellationToken: cancellationToken);
    }

    private static void UpdateAuditEntities(DbContext? context)
    {
        if (context is null)
            return;

        DateTime utcNow = DateTime.UtcNow;

        foreach (EntityEntry<AuditableEntity> entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            Guid userId = entry.Entity.CreatedBy;

            if (userId == Guid.Empty && entry.Entity is Category category)
                userId = category.UserId;

            switch (entry.State)
            {
                case EntityState.Added:
                    {
                        if (entry.Property(e => e.CreatedAt).CurrentValue == default)
                            entry.Property(e => e.CreatedAt).CurrentValue = utcNow;

                        entry.Property(e => e.UpdatedAt).CurrentValue = utcNow;

                        if (userId != Guid.Empty)
                        {
                            if (entry.Property(e => e.CreatedBy).CurrentValue == Guid.Empty)
                                entry.Property(e => e.CreatedBy).CurrentValue = userId;

                            entry.Property(e => e.UpdatedBy).CurrentValue = userId;
                        }

                        break;
                    }
                case EntityState.Modified:
                    {
                        entry.Property(e => e.UpdatedAt).CurrentValue = utcNow;

                        Guid updateUserId = entry.Entity.UpdatedBy ?? userId;
                        if (updateUserId != Guid.Empty)
                            entry.Property(e => e.UpdatedBy).CurrentValue = updateUserId;
                        break;
                    }

                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                case EntityState.Deleted:
                    break;
                default:
                    break;
            }
        }
    }
}
