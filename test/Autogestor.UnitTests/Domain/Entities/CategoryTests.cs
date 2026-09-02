using Autogestor.Domain.Entities;

namespace Autogestor.UnitTests.Domain.Entities;

public class CategoryTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsValidCategory()
    {
        // Arrange
        string title = "Test Category";
        string description = "Test Description";
        var userId = Guid.NewGuid();

        // Act
        var category = Category.Create(title, description, userId);

        // Assert
        Assert.Equal(title, category.Title);
        Assert.Equal(description, category.Description);
        Assert.Equal(userId, category.UserId);
        Assert.True(category.Active); // Verify default state inherited from AuditableEntity
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidTitle_ThrowsArgumentException(string? invalidTitle)
    {
        // Arrange
        string description = "Test Description";
        var userId = Guid.NewGuid();

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => Category.Create(invalidTitle!, description, userId));
        Assert.Equal("title", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidDescription_ThrowsArgumentException(string? invalidDescription)
    {
        // Arrange
        string title = "Test Category";
        var userId = Guid.NewGuid();

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => Category.Create(title, invalidDescription!, userId));
        Assert.Equal("description", exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyUserId_ThrowsArgumentException()
    {
        // Arrange
        string title = "Test Category";
        string description = "Test Description";
        Guid userId = Guid.Empty;

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => Category.Create(title, description, userId));
        Assert.Equal("userId", exception.ParamName);
    }
}
