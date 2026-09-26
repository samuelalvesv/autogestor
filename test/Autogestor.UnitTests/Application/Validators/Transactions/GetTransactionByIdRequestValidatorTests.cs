using Autogestor.Application.Validators.Transactions;
using Autogestor.Contract.Requests.Transactions;
using FluentValidation.Results;

namespace Autogestor.UnitTests.Application.Validators.Transactions;

public sealed class GetTransactionByIdRequestValidatorTests
{
    private readonly GetTransactionByIdRequestValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_WithValidId_ReturnsValidResult()
    {
        // Arrange
        var request = new GetTransactionByIdRequest
        {
            Id = Guid.NewGuid()
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
        var request = new GetTransactionByIdRequest
        {
            Id = Guid.Empty
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: result.IsValid);
        Assert.Contains(collection: result.Errors, filter: static e => e.ErrorMessage == "O identificador da transação é obrigatório.");
    }
}
