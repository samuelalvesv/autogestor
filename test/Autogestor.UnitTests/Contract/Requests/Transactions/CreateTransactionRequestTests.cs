using Autogestor.Contract.Enums;
using Autogestor.Contract.Requests.Transactions;

namespace Autogestor.UnitTests.Contract.Requests.Transactions;

public sealed class CreateTransactionRequestTests
{
    [Fact]
    public void CreateTransactionRequest_WithValues_SetsPropertiesCorrectly()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        // Act
        var request = new CreateTransactionRequest
        {
            Title = "Venda de Veículo",
            Type = ETransactionType.Deposit,
            Amount = 45000.00m,
            CategoryId = categoryId
        };

        // Assert
        Assert.Equal(expected: "Venda de Veículo", actual: request.Title);
        Assert.Equal(expected: ETransactionType.Deposit, actual: request.Type);
        Assert.Equal(expected: 45000.00m, actual: request.Amount);
        Assert.Equal(expected: categoryId, actual: request.CategoryId);
    }

    [Fact]
    public void CreateTransactionRequest_RecordEquality_ReturnsTrueForEqualValues()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request1 = new CreateTransactionRequest { Title = "A", Type = ETransactionType.Deposit, Amount = 100m, CategoryId = categoryId };
        var request2 = new CreateTransactionRequest { Title = "A", Type = ETransactionType.Deposit, Amount = 100m, CategoryId = categoryId };

        // Act & Assert
        Assert.Equal(expected: request1, actual: request2);
    }
}
