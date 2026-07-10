using Autogestor.Domain.Entities;
using Autogestor.Domain.Enums;

namespace Autogestor.UnitTests.Entities;

public class TransactionTests
{
    [Fact]
    public void Create_ReturnsValidTransaction()
    {
        // Arrange
        string name = "Test Transaction";
        ETransactionType type = ETransactionType.Withdraw;
        decimal amount = 100.00m;
        Guid categoryId = Guid.NewGuid();

        // Act
        var transaction = Transaction.Create(name, type, amount, categoryId);

        // Assert
        Assert.Equal(name, transaction.Name);
        Assert.Equal(type, transaction.Type);
        Assert.Equal(amount, transaction.Amount);
        Assert.Equal(categoryId, transaction.CategoryId);
    }

    [Fact]
    public void Create_ThrowsArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        string name = "";
        ETransactionType type = ETransactionType.Withdraw;
        decimal amount = 100.00m;
        Guid categoryId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Transaction.Create(name, type, amount, categoryId));
    }

    [Fact]
    public void Create_ThrowsArgumentException_WhenAmountIsZeroOrLess()
    {
        // Arrange
        string name = "Test Transaction";
        ETransactionType type = ETransactionType.Withdraw;
        decimal amount = 0.00m;
        Guid categoryId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Transaction.Create(name, type, amount, categoryId));
    }

    [Fact]
    public void Create_ThrowsArgumentException_WhenCategoryIdIsEmpty()
    {
        // Arrange
        string name = "Test Transaction";
        ETransactionType type = ETransactionType.Withdraw;
        decimal amount = 100.00m;
        Guid categoryId = Guid.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Transaction.Create(name, type, amount, categoryId));
    }
}
