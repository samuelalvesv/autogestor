using Autogestor.Contract.Requests.Categories;

namespace Autogestor.UnitTests.Contract.Requests.Categories;

public sealed class DeleteCategoryRequestTests
{
    [Fact]
    public void DeleteCategoryRequest_WithValues_SetsPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var request = new DeleteCategoryRequest
        {
            Id = id
        };

        // Assert
        Assert.Equal(expected: id, actual: request.Id);
    }

    [Fact]
    public void DeleteCategoryRequest_RecordEquality_ReturnsTrueForEqualValues()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request1 = new DeleteCategoryRequest { Id = id };
        var request2 = new DeleteCategoryRequest { Id = id };

        // Act & Assert
        Assert.Equal(expected: request1, actual: request2);
    }
}
