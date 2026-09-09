using Autogestor.Domain.Interfaces;

namespace Autogestor.IntegrationTests.Fixtures;

public sealed class TenantContextFake(Guid tenantId) : ITenantContext
{
    public TenantContextFake() : this(tenantId: Guid.NewGuid())
    {
    }

    public Guid TenantId { get; set; } = tenantId;
}
