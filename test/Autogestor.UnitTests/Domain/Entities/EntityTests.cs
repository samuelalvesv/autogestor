using Autogestor.Domain.Entities;

namespace Autogestor.UnitTests.Domain.Entities;

public class EntityTests
{
    private class TestEntity : Entity
    {
        // Simple concrete implementation to test the abstract Entity class
    }

    [Fact]
    public void Constructor_ShouldInitializeWithNonEmptyGuid()
    {
        // Act
        var entity = new TestEntity();

        // Assert
        Assert.NotEqual(Guid.Empty, entity.Id);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithUniqueGuids()
    {
        // Act
        var entity1 = new TestEntity();
        var entity2 = new TestEntity();

        // Assert
        Assert.NotEqual(entity1.Id, entity2.Id);
    }
}
