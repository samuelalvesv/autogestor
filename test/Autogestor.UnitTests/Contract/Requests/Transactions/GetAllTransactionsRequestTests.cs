using Autogestor.Contract.Requests.Transactions;

namespace Autogestor.UnitTests.Contract.Requests.Transactions;

public sealed class GetAllTransactionsRequestTests
{
    [Fact]
    public void GetAllTransactionsRequest_WithValues_SetsPropertiesCorrectly()
    {
        // Arrange
        var cursor = Guid.NewGuid();

        // Act
        var request = new GetAllTransactionsRequest
        {
            Cursor = cursor,
            PageSize = 50
        };

        // Assert
        Assert.Equal(expected: cursor, actual: request.Cursor);
        Assert.Equal(expected: 50, actual: request.PageSize);
    }

    [Fact]
    public void GetAllTransactionsRequest_RecordEquality_ReturnsTrueForEqualValues()
    {
        // Arrange
        var cursor = Guid.NewGuid();
        var request1 = new GetAllTransactionsRequest { Cursor = cursor, PageSize = 25 };
        var request2 = new GetAllTransactionsRequest { Cursor = cursor, PageSize = 25 };

        // Act & Assert
        Assert.Equal(expected: request1, actual: request2);
    }
}
