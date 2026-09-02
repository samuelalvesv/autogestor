using Autogestor.Domain.Entities;
using Autogestor.Domain.Enums;

namespace Autogestor.UnitTests.Domain.Entities;

public class TransactionTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsValidTransaction()
    {
        // Arrange
        string title = "Test Transaction";
        ETransactionType type = ETransactionType.Withdraw;
        decimal amount = 100.00m;
        var categoryId = Guid.NewGuid();

        // Act
        var transaction = Transaction.Create(title, type, amount, categoryId);

        // Assert
        Assert.Equal(title, transaction.Title);
        Assert.Equal(type, transaction.Type);
        Assert.Equal(amount, transaction.Amount);
        Assert.Equal(categoryId, transaction.CategoryId);
        Assert.True(transaction.Active); // Verify default state inherited from AuditableEntity
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidTitle_ThrowsArgumentException(string? invalidTitle)
    {
        // Arrange
        ETransactionType type = ETransactionType.Withdraw;
        decimal amount = 100.00m;
        var categoryId = Guid.NewGuid();

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => Transaction.Create(invalidTitle!, type, amount, categoryId));
        Assert.Equal("title", exception.ParamName);
    }

    [Theory]
    [InlineData(0.00)]
    [InlineData(-1.00)]
    [InlineData(-99.99)]
    public void Create_WithZeroOrNegativeAmount_ThrowsArgumentException(double invalidAmountDouble)
    {
        // Arrange
        string title = "Test Transaction";
        ETransactionType type = ETransactionType.Withdraw;
        decimal invalidAmount = (decimal)invalidAmountDouble;
        var categoryId = Guid.NewGuid();

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => Transaction.Create(title, type, invalidAmount, categoryId));
        Assert.Equal("amount", exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyCategoryId_ThrowsArgumentException()
    {
        // Arrange
        string title = "Test Transaction";
        ETransactionType type = ETransactionType.Withdraw;
        decimal amount = 100.00m;
        Guid categoryId = Guid.Empty;

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => Transaction.Create(title, type, amount, categoryId));
        Assert.Equal("categoryId", exception.ParamName);
    }
}
