using Autogestor.Contract.Enums;
using Autogestor.Contract.Requests.Transactions;

namespace Autogestor.UnitTests.Contract.Requests.Transactions;

public sealed class UpdateTransactionRequestTests
{
    [Fact]
    public void UpdateTransactionRequest_WithValues_SetsPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        // Act
        var request = new UpdateTransactionRequest
        {
            Id = id,
            Title = "Compra de Peças",
            Type = ETransactionType.Withdraw,
            Amount = 1500.50m,
            CategoryId = categoryId
        };

        // Assert
        Assert.Equal(expected: id, actual: request.Id);
        Assert.Equal(expected: "Compra de Peças", actual: request.Title);
        Assert.Equal(expected: ETransactionType.Withdraw, actual: request.Type);
        Assert.Equal(expected: 1500.50m, actual: request.Amount);
        Assert.Equal(expected: categoryId, actual: request.CategoryId);
    }

    [Fact]
    public void UpdateTransactionRequest_RecordEquality_ReturnsTrueForEqualValues()
    {
        // Arrange
        var id = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var request1 = new UpdateTransactionRequest { Id = id, Title = "A", Type = ETransactionType.Withdraw, Amount = 100m, CategoryId = categoryId };
        var request2 = new UpdateTransactionRequest { Id = id, Title = "A", Type = ETransactionType.Withdraw, Amount = 100m, CategoryId = categoryId };

        // Act & Assert
        Assert.Equal(expected: request1, actual: request2);
    }
}
