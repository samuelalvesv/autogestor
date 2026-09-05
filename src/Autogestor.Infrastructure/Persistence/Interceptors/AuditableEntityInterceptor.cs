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
        Guid currentUserId = userContext.UserId;

        foreach (EntityEntry<AuditableEntity> entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    {
                        if (entry.Property(e => e.CreatedAt).CurrentValue == default)
                            entry.Property(e => e.CreatedAt).CurrentValue = utcNow;

                        entry.Property(e => e.UpdatedAt).CurrentValue = utcNow;

                        if (entry.Entity.CreatedBy == Guid.Empty)
                            entry.Property(e => e.CreatedBy).CurrentValue = currentUserId;

                        entry.Property(e => e.UpdatedBy).CurrentValue = currentUserId;
                        break;
                    }
                case EntityState.Modified:
                    {
                        entry.Property(e => e.UpdatedAt).CurrentValue = utcNow;
                        entry.Property(e => e.UpdatedBy).CurrentValue = currentUserId;
                        break;
                    }

                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                default:
                    break;
            }
        }
    }
}
