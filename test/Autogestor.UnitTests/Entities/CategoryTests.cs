using Autogestor.Domain.Entities;

namespace Autogestor.UnitTests.Entities;

public class CategoryTests
{
    [Fact]
    public void Create_ReturnsValidCategory()
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
    }

    [Fact]
    public void Create_ThrowsArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        string name = "";
        string description = "Test Description";
        Guid userId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Category.Create(name, description, userId));
    }

    [Fact]
    public void Create_ThrowsArgumentException_WhenDescriptionIsEmpty()
    {
        // Arrange
        string name = "Test Category";
        string description = "";
        Guid userId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Category.Create(name, description, userId));
    }

    [Fact]
    public void Create_ThrowsArgumentException_WhenUserIdIsEmpty()
    {
        // Arrange
        string name = "Test Category";
        string description = "Test Description";
        Guid userId = Guid.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Category.Create(name, description, userId));
    }
}
