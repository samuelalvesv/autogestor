using Autogestor.Application.Validators.Categories;
using Autogestor.Contract.Requests.Categories;
using FluentValidation.Results;

namespace Autogestor.UnitTests.Application.Validators.Categories;

public sealed class UpdateCategoryRequestValidatorTests
{
    private readonly UpdateCategoryRequestValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_WithValidRequest_ReturnsValidResult()
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.NewGuid(),
            Title = "Educação Superior",
            Description = "Cursos de graduação e pós-graduação"
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.True(condition: result.IsValid, userMessage: "A requisição válida deve passar na validação.");
        Assert.Empty(collection: result.Errors);
    }

    [Fact]
    public async Task ValidateAsync_WithEmptyId_ReturnsError()
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.Empty,
            Title = "Título válido",
            Description = "Descrição válida"
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: result.IsValid);
        Assert.Contains(collection: result.Errors, filter: static e => e.ErrorMessage == "O identificador da categoria é obrigatório.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ValidateAsync_WithInvalidTitle_ReturnsError(string? title)
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.NewGuid(),
            Title = title!,
            Description = "Descrição válida"
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: result.IsValid);
        Assert.Contains(collection: result.Errors, filter: static e => e.PropertyName == nameof(UpdateCategoryRequest.Title));
    }
}
