namespace Autogestor.Domain.Entities;

public abstract class TenantEntity : AuditableEntity
{
    public Guid TenantId { get; protected set; }
}
