using Autogestor.Contract.Requests.Categories;

namespace Autogestor.UnitTests.Contract.Requests.Categories;

public sealed class GetAllCategoriesRequestTests
{
    [Fact]
    public void GetAllCategoriesRequest_WithValues_SetsPropertiesCorrectly()
    {
        // Arrange
        var cursor = Guid.NewGuid();

        // Act
        var request = new GetAllCategoriesRequest
        {
            Cursor = cursor,
            PageSize = 50
        };

        // Assert
        Assert.Equal(expected: cursor, actual: request.Cursor);
        Assert.Equal(expected: 50, actual: request.PageSize);
    }

    [Fact]
    public void GetAllCategoriesRequest_RecordEquality_ReturnsTrueForEqualValues()
    {
        // Arrange
        var cursor = Guid.NewGuid();
        var request1 = new GetAllCategoriesRequest { Cursor = cursor, PageSize = 25 };
        var request2 = new GetAllCategoriesRequest { Cursor = cursor, PageSize = 25 };

        // Act & Assert
        Assert.Equal(expected: request1, actual: request2);
    }
}
