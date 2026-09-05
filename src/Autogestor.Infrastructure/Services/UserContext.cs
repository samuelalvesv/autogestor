using Autogestor.Domain.Interfaces;

namespace Autogestor.Infrastructure.Services;

public sealed class UserContext : IUserContext
{
    public Guid UserId => throw new InvalidOperationException("Nenhum usuário autenticado no contexto atual.");
}
