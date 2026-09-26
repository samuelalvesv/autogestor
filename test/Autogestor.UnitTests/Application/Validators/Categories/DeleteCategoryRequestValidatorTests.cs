using Autogestor.Application.Validators.Categories;
using Autogestor.Contract.Requests.Categories;
using FluentValidation.Results;

namespace Autogestor.UnitTests.Application.Validators.Categories;

public sealed class DeleteCategoryRequestValidatorTests
{
    private readonly DeleteCategoryRequestValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_WithValidId_ReturnsValidResult()
    {
        // Arrange
        var request = new DeleteCategoryRequest
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
        var request = new DeleteCategoryRequest
        {
            Id = Guid.Empty
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: result.IsValid);
        Assert.Contains(collection: result.Errors, filter: static e => e.ErrorMessage == "O identificador da categoria é obrigatório.");
    }
}
