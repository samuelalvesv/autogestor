using Autogestor.Domain.Entities;

namespace Autogestor.UnitTests.Domain.Entities;

public sealed class AuditableEntityTests
{
    private sealed class TestAuditableEntity : AuditableEntity
    {
    }

    [Fact]
    public void Constructor_ShouldInitializeActiveAsTrue()
    {
        // Act
        var entity = new TestAuditableEntity();

        // Assert
        Assert.True(condition: entity.Active, userMessage: "A entidade deve iniciar como ativa.");
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
        Assert.True(condition: entity.Active, userMessage: "A entidade deve estar ativa.");
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
        Assert.False(condition: entity.Active, userMessage: "A entidade deve estar inativa.");
    }
}
