using Autogestor.Contract.Requests.Transactions;

namespace Autogestor.UnitTests.Contract.Requests.Transactions;

public sealed class GetTransactionByIdRequestTests
{
    [Fact]
    public void GetTransactionByIdRequest_WithValues_SetsPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var request = new GetTransactionByIdRequest
        {
            Id = id
        };

        // Assert
        Assert.Equal(expected: id, actual: request.Id);
    }

    [Fact]
    public void GetTransactionByIdRequest_RecordEquality_ReturnsTrueForEqualValues()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request1 = new GetTransactionByIdRequest { Id = id };
        var request2 = new GetTransactionByIdRequest { Id = id };

        // Act & Assert
        Assert.Equal(expected: request1, actual: request2);
    }
}
