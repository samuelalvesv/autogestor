using Autogestor.Contract.Requests.Categories;

namespace Autogestor.UnitTests.Contract.Requests.Categories;

public sealed class CreateCategoryRequestTests
{
    [Fact]
    public void CreateCategoryRequest_WithValues_SetsPropertiesCorrectly()
    {
        // Act
        var request = new CreateCategoryRequest
        {
            Title = "Investimentos",
            Description = "Categoria para despesas de investimento"
        };

        // Assert
        Assert.Equal(expected: "Investimentos", actual: request.Title);
        Assert.Equal(expected: "Categoria para despesas de investimento", actual: request.Description);
    }

    [Fact]
    public void CreateCategoryRequest_RecordEquality_ReturnsTrueForEqualValues()
    {
        // Arrange
        var request1 = new CreateCategoryRequest { Title = "A", Description = "B" };
        var request2 = new CreateCategoryRequest { Title = "A", Description = "B" };

        // Act & Assert
        Assert.Equal(expected: request1, actual: request2);
    }
}
