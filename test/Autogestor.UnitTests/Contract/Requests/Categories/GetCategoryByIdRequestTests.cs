using Autogestor.Contract.Requests.Categories;

namespace Autogestor.UnitTests.Contract.Requests.Categories;

public sealed class GetCategoryByIdRequestTests
{
    [Fact]
    public void GetCategoryByIdRequest_WithValues_SetsPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var request = new GetCategoryByIdRequest
        {
            Id = id
        };

        // Assert
        Assert.Equal(expected: id, actual: request.Id);
    }

    [Fact]
    public void GetCategoryByIdRequest_RecordEquality_ReturnsTrueForEqualValues()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request1 = new GetCategoryByIdRequest { Id = id };
        var request2 = new GetCategoryByIdRequest { Id = id };

        // Act & Assert
        Assert.Equal(expected: request1, actual: request2);
    }
}
