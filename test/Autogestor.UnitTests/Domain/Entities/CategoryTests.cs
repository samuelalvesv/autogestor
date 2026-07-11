using Autogestor.Domain.Entities;

namespace Autogestor.UnitTests.Domain.Entities;

public class CategoryTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsValidCategory()
    {
        // Arrange
        string name = "Test Category";
        string description = "Test Description";
        Guid userId = Guid.NewGuid();

        // Act
        var category = Category.Create(name, description, userId);

        // Assert
        Assert.Equal(name, category.Name);
        Assert.Equal(description, category.Description);
        Assert.Equal(userId, category.UserId);
        Assert.True(category.Active); // Verify default state inherited from AuditableEntity
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidName_ThrowsArgumentException(string? invalidName)
    {
        // Arrange
        string description = "Test Description";
        Guid userId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Category.Create(invalidName!, description, userId));
        Assert.Equal("name", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidDescription_ThrowsArgumentException(string? invalidDescription)
    {
        // Arrange
        string name = "Test Category";
        Guid userId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Category.Create(name, invalidDescription!, userId));
        Assert.Equal("description", exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyUserId_ThrowsArgumentException()
    {
        // Arrange
        string name = "Test Category";
        string description = "Test Description";
        Guid userId = Guid.Empty;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Category.Create(name, description, userId));
        Assert.Equal("userId", exception.ParamName);
    }
}
