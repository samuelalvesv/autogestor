using Autogestor.Contract.Requests.Categories;

namespace Autogestor.UnitTests.Contract.Requests.Categories;

public sealed class UpdateCategoryRequestTests
{
    [Fact]
    public void UpdateCategoryRequest_WithValues_SetsPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var request = new UpdateCategoryRequest
        {
            Id = id,
            Title = "Alimentação",
            Description = "Gastos com restaurantes e supermercado"
        };

        // Assert
        Assert.Equal(expected: id, actual: request.Id);
        Assert.Equal(expected: "Alimentação", actual: request.Title);
        Assert.Equal(expected: "Gastos com restaurantes e supermercado", actual: request.Description);
    }

    [Fact]
    public void UpdateCategoryRequest_RecordEquality_ReturnsTrueForEqualValues()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request1 = new UpdateCategoryRequest { Id = id, Title = "A", Description = "B" };
        var request2 = new UpdateCategoryRequest { Id = id, Title = "A", Description = "B" };

        // Act & Assert
        Assert.Equal(expected: request1, actual: request2);
    }
}
