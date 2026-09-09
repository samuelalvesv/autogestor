using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Autogestor.Infrastructure.Persistence.Interceptors;

public sealed class TenantEntityInterceptor(ITenantContext tenantContext) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateTenantEntities(context: eventData.Context!);
        return base.SavingChanges(eventData: eventData, result: result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateTenantEntities(context: eventData.Context!);
        return base.SavingChangesAsync(
            eventData: eventData,
            result: result,
            cancellationToken: cancellationToken);
    }

    private void UpdateTenantEntities(DbContext context)
    {
        Guid currentTenantId = tenantContext.TenantId;

        foreach (EntityEntry<TenantEntity> entry in context.ChangeTracker.Entries<TenantEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    {
                        if (entry.Entity.TenantId == Guid.Empty)
                            entry.Property(e => e.TenantId).CurrentValue = currentTenantId;

                        break;
                    }
                case EntityState.Modified:
                    {
                        entry.Property(e => e.TenantId).IsModified = false;
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
