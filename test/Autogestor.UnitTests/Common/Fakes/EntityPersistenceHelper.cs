using Autogestor.Domain.Entities;

namespace Autogestor.UnitTests.Common.Fakes;

public static class EntityPersistenceHelper
{
    public static void SetPersistenceFields(TenantEntity entity, Guid userId, Guid tenantId, DateTime timestamp)
    {
        typeof(AuditableEntity).GetProperty(name: nameof(AuditableEntity.CreatedBy))!
            .SetValue(obj: entity, value: userId);
        typeof(AuditableEntity).GetProperty(name: nameof(AuditableEntity.CreatedAt))!
            .SetValue(obj: entity, value: timestamp);
        typeof(TenantEntity).GetProperty(name: nameof(TenantEntity.TenantId))!
            .SetValue(obj: entity, value: tenantId);
    }
}
