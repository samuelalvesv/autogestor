using Autogestor.Application.Validators;
using Autogestor.Application.Validators.Categories;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Domain.Exceptions;

namespace Autogestor.UnitTests.Application.Validators;

public sealed class ValidatorExtensionsTests
{
    private readonly CreateCategoryRequestValidator _validator = new();

    [Fact]
    public async Task ValidateOrThrowAsync_WithValidInstance_DoesNotThrow()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Title = "Alimentação",
            Description = "Despesas com supermercado e restaurantes"
        };

        // Act
        Exception? exception = await Record.ExceptionAsync(
            testCode: () => _validator.ValidateOrThrowAsync(
                instance: request,
                cancellationToken: TestContext.Current.CancellationToken));

        // Assert
        Assert.Null(@object: exception);
    }

    [Fact]
    public async Task ValidateOrThrowAsync_WithSingleError_ThrowsDomainValidationExceptionWithSingleMessage()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Title = "AB",
            Description = "Descrição válida"
        };

        // Act & Assert
        DomainValidationException exception = await Assert.ThrowsAsync<DomainValidationException>(
            testCode: () => _validator.ValidateOrThrowAsync(
                instance: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(
            expected: "O título deve conter entre 3 e 80 caracteres.",
            actual: exception.Message);
    }

    [Fact]
    public async Task ValidateOrThrowAsync_WithMultipleErrors_ThrowsDomainValidationExceptionWithJoinedMessages()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Title = "AB",
            Description = "CD"
        };

        // Act & Assert
        DomainValidationException exception = await Assert.ThrowsAsync<DomainValidationException>(
            testCode: () => _validator.ValidateOrThrowAsync(
                instance: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Contains(
            expectedSubstring: ", ",
            actualString: exception.Message,
            comparisonType: StringComparison.Ordinal);
        Assert.Contains(
            expectedSubstring: "O título deve conter entre 3 e 80 caracteres.",
            actualString: exception.Message,
            comparisonType: StringComparison.Ordinal);
        Assert.Contains(
            expectedSubstring: "A descrição deve conter entre 3 e 180 caracteres.",
            actualString: exception.Message,
            comparisonType: StringComparison.Ordinal);
        Assert.Equal(
            expected: "O título deve conter entre 3 e 80 caracteres., A descrição deve conter entre 3 e 180 caracteres.",
            actual: exception.Message);
    }

    [Fact]
    public async Task ValidateOrThrowAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Title = "Alimentação",
            Description = "Descrição válida"
        };

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            testCode: () => _validator.ValidateOrThrowAsync(
                instance: request,
                cancellationToken: cts.Token));
    }
}
