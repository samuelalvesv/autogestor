namespace Autogestor.Domain.Interfaces;

public interface ITenantContext
{
    Guid TenantId { get; }
}
