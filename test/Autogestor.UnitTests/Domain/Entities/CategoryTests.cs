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

        // Act
        var category = Category.Create(title, description);

        // Assert
        Assert.Equal(title, category.Title);
        Assert.Equal(description, category.Description);
        Assert.True(category.Active); // Verify default state inherited from AuditableEntity
        Assert.IsAssignableFrom<TenantEntity>(category);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidTitle_ThrowsArgumentException(string? invalidTitle)
    {
        // Arrange
        string description = "Test Description";

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => Category.Create(invalidTitle!, description));
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

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => Category.Create(title, invalidDescription!));
        Assert.Equal("description", exception.ParamName);
    }
}
