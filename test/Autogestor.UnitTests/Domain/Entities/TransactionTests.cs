using Autogestor.Domain.Entities;
using Autogestor.Domain.Enums;

namespace Autogestor.UnitTests.Domain.Entities;

public class TransactionTests
{
    [Theory]
    [InlineData(ETransactionType.Deposit)]
    [InlineData(ETransactionType.Withdraw)]
    public void Create_WithValidParameters_ReturnsValidTransaction(ETransactionType type)
    {
        // Arrange
        string title = "Test Transaction";
        decimal amount = 100.00m;
        var categoryId = Guid.NewGuid();

        // Act
        var transaction = Transaction.Create(
            title: title,
            type: type,
            amount: amount,
            categoryId: categoryId);

        // Assert
        Assert.Equal(expected: title, actual: transaction.Title);
        Assert.Equal(expected: type, actual: transaction.Type);
        Assert.Equal(expected: amount, actual: transaction.Amount);
        Assert.Equal(expected: categoryId, actual: transaction.CategoryId);
        Assert.True(condition: transaction.Active, userMessage: "A transação deve ser criada como ativa por padrão.");
        Assert.IsAssignableFrom<TenantEntity>(@object: transaction);
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
        ArgumentException exception = Assert.Throws<ArgumentException>(
            testCode: () => Transaction.Create(
                title: invalidTitle!,
                type: type,
                amount: amount,
                categoryId: categoryId));
        Assert.Equal(expected: "title", actual: exception.ParamName);
        Assert.Contains(expectedSubstring: "O título da transação não pode ser vazio.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
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
        ArgumentException exception = Assert.Throws<ArgumentException>(
            testCode: () => Transaction.Create(
                title: title,
                type: type,
                amount: invalidAmount,
                categoryId: categoryId));
        Assert.Equal(expected: "amount", actual: exception.ParamName);
        Assert.Contains(expectedSubstring: "O valor da transação deve ser maior que zero.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
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
        ArgumentException exception = Assert.Throws<ArgumentException>(
            testCode: () => Transaction.Create(
                title: title,
                type: type,
                amount: amount,
                categoryId: categoryId));
        Assert.Equal(expected: "categoryId", actual: exception.ParamName);
        Assert.Contains(expectedSubstring: "Categoria inválida.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
    }

    [Theory]
    [InlineData((ETransactionType)0)]
    [InlineData((ETransactionType)99)]
    [InlineData((ETransactionType)(-1))]
    public void Create_WithInvalidType_ThrowsArgumentException(ETransactionType invalidType)
    {
        // Arrange
        string title = "Test Transaction";
        decimal amount = 100.00m;
        var categoryId = Guid.NewGuid();

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(
            testCode: () => Transaction.Create(
                title: title,
                type: invalidType,
                amount: amount,
                categoryId: categoryId));
        Assert.Equal(expected: "type", actual: exception.ParamName);
        Assert.Contains(expectedSubstring: "Tipo de transação inválido.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
    }

    [Fact]
    public void Update_WithValidParameters_UpdatesProperties()
    {
        // Arrange
        var transaction = Transaction.Create(
            title: "Original Title",
            type: ETransactionType.Deposit,
            amount: 50.00m,
            categoryId: Guid.NewGuid());

        string updatedTitle = "Updated Title";
        ETransactionType updatedType = ETransactionType.Withdraw;
        decimal updatedAmount = 250.00m;
        var updatedCategoryId = Guid.NewGuid();

        // Act
        transaction.Update(
            title: updatedTitle,
            type: updatedType,
            amount: updatedAmount,
            categoryId: updatedCategoryId);

        // Assert
        Assert.Equal(expected: updatedTitle, actual: transaction.Title);
        Assert.Equal(expected: updatedType, actual: transaction.Type);
        Assert.Equal(expected: updatedAmount, actual: transaction.Amount);
        Assert.Equal(expected: updatedCategoryId, actual: transaction.CategoryId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Update_WithInvalidTitle_ThrowsArgumentException(string? invalidTitle)
    {
        // Arrange
        var transaction = Transaction.Create(
            title: "Original Title",
            type: ETransactionType.Deposit,
            amount: 50.00m,
            categoryId: Guid.NewGuid());

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(
            testCode: () => transaction.Update(
                title: invalidTitle!,
                type: ETransactionType.Withdraw,
                amount: 100.00m,
                categoryId: Guid.NewGuid()));
        Assert.Equal(expected: "title", actual: exception.ParamName);
        Assert.Contains(expectedSubstring: "O título da transação não pode ser vazio.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
    }

    [Theory]
    [InlineData((ETransactionType)0)]
    [InlineData((ETransactionType)99)]
    [InlineData((ETransactionType)(-1))]
    public void Update_WithInvalidType_ThrowsArgumentException(ETransactionType invalidType)
    {
        // Arrange
        var transaction = Transaction.Create(
            title: "Original Title",
            type: ETransactionType.Deposit,
            amount: 50.00m,
            categoryId: Guid.NewGuid());

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(
            testCode: () => transaction.Update(
                title: "Updated Title",
                type: invalidType,
                amount: 100.00m,
                categoryId: Guid.NewGuid()));
        Assert.Equal(expected: "type", actual: exception.ParamName);
        Assert.Contains(expectedSubstring: "Tipo de transação inválido.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(0.00)]
    [InlineData(-1.00)]
    [InlineData(-99.99)]
    public void Update_WithZeroOrNegativeAmount_ThrowsArgumentException(double invalidAmountDouble)
    {
        // Arrange
        var transaction = Transaction.Create(
            title: "Original Title",
            type: ETransactionType.Deposit,
            amount: 50.00m,
            categoryId: Guid.NewGuid());

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(
            testCode: () => transaction.Update(
                title: "Updated Title",
                type: ETransactionType.Withdraw,
                amount: (decimal)invalidAmountDouble,
                categoryId: Guid.NewGuid()));
        Assert.Equal(expected: "amount", actual: exception.ParamName);
        Assert.Contains(expectedSubstring: "O valor da transação deve ser maior que zero.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
    }

    [Fact]
    public void Update_WithEmptyCategoryId_ThrowsArgumentException()
    {
        // Arrange
        var transaction = Transaction.Create(
            title: "Original Title",
            type: ETransactionType.Deposit,
            amount: 50.00m,
            categoryId: Guid.NewGuid());

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(
            testCode: () => transaction.Update(
                title: "Updated Title",
                type: ETransactionType.Withdraw,
                amount: 100.00m,
                categoryId: Guid.Empty));
        Assert.Equal(expected: "categoryId", actual: exception.ParamName);
        Assert.Contains(expectedSubstring: "Categoria inválida.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
    }
}
