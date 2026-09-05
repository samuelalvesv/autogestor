using Autogestor.Domain.Interfaces;

namespace Autogestor.IntegrationTests.Fixtures;

public sealed class UserContextFake(Guid? userId = null) : IUserContext
{
    public Guid UserId { get; set; } = userId ?? Guid.NewGuid();
}
