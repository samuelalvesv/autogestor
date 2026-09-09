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
        var addedTenantEntries = context.ChangeTracker.Entries<TenantEntity>()
            .Where(predicate: e => e.State == EntityState.Added)
            .ToList();

        if (addedTenantEntries.Count > 0)
        {
            Guid currentTenantId = tenantContext.TenantId;
            if (currentTenantId == Guid.Empty)
                throw new InvalidOperationException(message: "Não é possível salvar entidades com escopo de tenant sem um identificador de tenant válido.");

            foreach (EntityEntry<TenantEntity> entry in addedTenantEntries)
            {
                entry.Property(propertyExpression: e => e.TenantId).CurrentValue = currentTenantId;
            }
        }

        foreach (EntityEntry<TenantEntity> entry in context.ChangeTracker.Entries<TenantEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Property(propertyExpression: e => e.TenantId).IsModified = false;
        }
    }
}
