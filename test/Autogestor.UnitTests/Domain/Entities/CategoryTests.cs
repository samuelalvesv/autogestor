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
        var category = Category.Create(title: title, description: description);

        // Assert
        Assert.Equal(expected: title, actual: category.Title);
        Assert.Equal(expected: description, actual: category.Description);
        Assert.True(condition: category.Active, userMessage: "A categoria deve ser criada como ativa por padrão.");
        Assert.IsAssignableFrom<TenantEntity>(@object: category);
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
        ArgumentException exception = Assert.Throws<ArgumentException>(
            testCode: () => Category.Create(title: invalidTitle!, description: description));
        Assert.Equal(expected: "title", actual: exception.ParamName);
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
        ArgumentException exception = Assert.Throws<ArgumentException>(
            testCode: () => Category.Create(title: title, description: invalidDescription!));
        Assert.Equal(expected: "description", actual: exception.ParamName);
    }
}
