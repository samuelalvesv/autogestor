using Autogestor.Contract.Requests.Transactions;

namespace Autogestor.UnitTests.Contract.Requests.Transactions;

public sealed class DeleteTransactionRequestTests
{
    [Fact]
    public void DeleteTransactionRequest_WithValues_SetsPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var request = new DeleteTransactionRequest
        {
            Id = id
        };

        // Assert
        Assert.Equal(expected: id, actual: request.Id);
    }

    [Fact]
    public void DeleteTransactionRequest_RecordEquality_ReturnsTrueForEqualValues()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request1 = new DeleteTransactionRequest { Id = id };
        var request2 = new DeleteTransactionRequest { Id = id };

        // Act & Assert
        Assert.Equal(expected: request1, actual: request2);
    }
}
