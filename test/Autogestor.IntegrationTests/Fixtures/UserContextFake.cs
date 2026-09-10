using Autogestor.Domain.Interfaces;

namespace Autogestor.IntegrationTests.Fixtures;

public sealed class UserContextFake(Guid userId) : IUserContext
{
    public UserContextFake() : this(userId: Guid.NewGuid())
    {
    }

    public Guid UserId { get; set; } = userId;
}
