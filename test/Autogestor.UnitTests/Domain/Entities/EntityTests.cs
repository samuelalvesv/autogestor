using Autogestor.Domain.Entities;

namespace Autogestor.UnitTests.Domain.Entities;

public sealed class EntityTests
{
    private sealed class TestEntity : Entity
    {
    }

    [Fact]
    public void Constructor_ShouldInitializeWithNonEmptyGuid()
    {
        // Act
        var entity = new TestEntity();

        // Assert
        Assert.NotEqual(expected: Guid.Empty, actual: entity.Id);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithUniqueGuids()
    {
        // Act
        var entity1 = new TestEntity();
        var entity2 = new TestEntity();

        // Assert
        Assert.NotEqual(expected: entity1.Id, actual: entity2.Id);
    }

    [Fact]
    public void Constructor_ShouldGenerateUuidVersion7()
    {
        // Act
        var entity = new TestEntity();

        // Assert
        Assert.Equal(expected: 7, actual: entity.Id.Version);
    }
}
