using Autogestor.Application.Validators.Transactions;
using Autogestor.Contract.Enums;
using Autogestor.Contract.Requests.Transactions;
using FluentValidation.Results;

namespace Autogestor.UnitTests.Application.Validators.Transactions;

public sealed class CreateTransactionRequestValidatorTests
{
    private readonly CreateTransactionRequestValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_WithValidRequest_ReturnsValidResult()
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            Title = "Salário Mensal",
            Type = ETransactionType.Deposit,
            Amount = 5000.00m,
            CategoryId = Guid.NewGuid()
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.True(condition: result.IsValid);
        Assert.Empty(collection: result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ValidateAsync_WithInvalidTitle_ReturnsError(string? title)
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            Title = title!,
            Type = ETransactionType.Deposit,
            Amount = 100m,
            CategoryId = Guid.NewGuid()
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: result.IsValid);
        Assert.Contains(collection: result.Errors, filter: static e => e.PropertyName == nameof(CreateTransactionRequest.Title));
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidEnumType_ReturnsError()
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            Title = "Investimento",
            Type = (ETransactionType)999,
            Amount = 100m,
            CategoryId = Guid.NewGuid()
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: result.IsValid);
        Assert.Contains(collection: result.Errors, filter: static e => e.ErrorMessage == "Tipo de transação inválido.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [InlineData(-100)]
    public async Task ValidateAsync_WithZeroOrNegativeAmount_ReturnsError(decimal amount)
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            Title = "Compra de Livros",
            Type = ETransactionType.Withdraw,
            Amount = amount,
            CategoryId = Guid.NewGuid()
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: result.IsValid);
        Assert.Contains(collection: result.Errors, filter: static e => e.ErrorMessage == "O valor da transação deve ser maior que zero.");
    }

    [Fact]
    public async Task ValidateAsync_WithEmptyCategoryId_ReturnsError()
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            Title = "Conta de Luz",
            Type = ETransactionType.Withdraw,
            Amount = 150m,
            CategoryId = Guid.Empty
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: result.IsValid);
        Assert.Contains(collection: result.Errors, filter: static e => e.ErrorMessage == "O identificador da categoria é obrigatório.");
    }
}
