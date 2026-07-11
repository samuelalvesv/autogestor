using Autogestor.Domain.Entities;

namespace Autogestor.UnitTests.Domain.Entities;

public class AuditableEntityTests
{
    private class TestAuditableEntity : AuditableEntity
    {
        // Simple concrete implementation to test the abstract AuditableEntity class
    }

    [Fact]
    public void Constructor_ShouldInitializeActiveAsTrue()
    {
        // Act
        var entity = new TestAuditableEntity();

        // Assert
        Assert.True(entity.Active);
    }

    [Fact]
    public void Activate_ShouldSetActiveToTrue()
    {
        // Arrange
        var entity = new TestAuditableEntity();
        entity.Deactivate();

        // Act
        entity.Activate();

        // Assert
        Assert.True(entity.Active);
    }

    [Fact]
    public void Deactivate_ShouldSetActiveToFalse()
    {
        // Arrange
        var entity = new TestAuditableEntity();
        entity.Activate(); // Make sure it starts as true

        // Act
        entity.Deactivate();

        // Assert
        Assert.False(entity.Active);
    }
}
