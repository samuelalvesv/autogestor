using Autogestor.Domain.Interfaces;

namespace Autogestor.Infrastructure.Services;

public sealed class TenantContext : ITenantContext
{
    public Guid TenantId => throw new InvalidOperationException("Nenhum tenant associado ao contexto atual.");
}
