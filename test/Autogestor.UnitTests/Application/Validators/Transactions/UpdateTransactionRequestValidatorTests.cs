using Autogestor.Application.Validators.Transactions;
using Autogestor.Contract.Enums;
using Autogestor.Contract.Requests.Transactions;
using FluentValidation.Results;

namespace Autogestor.UnitTests.Application.Validators.Transactions;

public sealed class UpdateTransactionRequestValidatorTests
{
    private readonly UpdateTransactionRequestValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_WithValidRequest_ReturnsValidResult()
    {
        // Arrange
        var request = new UpdateTransactionRequest
        {
            Id = Guid.NewGuid(),
            Title = "Salário Reajustado",
            Type = ETransactionType.Deposit,
            Amount = 6000m,
            CategoryId = Guid.NewGuid()
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.True(condition: result.IsValid);
        Assert.Empty(collection: result.Errors);
    }

    [Fact]
    public async Task ValidateAsync_WithEmptyId_ReturnsError()
    {
        // Arrange
        var request = new UpdateTransactionRequest
        {
            Id = Guid.Empty,
            Title = "Título válido",
            Type = ETransactionType.Deposit,
            Amount = 100m,
            CategoryId = Guid.NewGuid()
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: result.IsValid);
        Assert.Contains(collection: result.Errors, filter: static e => e.ErrorMessage == "O identificador da transação é obrigatório.");
    }
}
