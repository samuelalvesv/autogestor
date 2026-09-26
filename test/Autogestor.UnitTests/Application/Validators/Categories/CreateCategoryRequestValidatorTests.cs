using Autogestor.Application.Validators.Categories;
using Autogestor.Contract.Requests.Categories;
using FluentValidation.Results;

namespace Autogestor.UnitTests.Application.Validators.Categories;

public sealed class CreateCategoryRequestValidatorTests
{
    private readonly CreateCategoryRequestValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_WithValidRequest_ReturnsValidResult()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Title = "Alimentação",
            Description = "Despesas com supermercado e restaurantes"
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.True(condition: result.IsValid, userMessage: "A requisição válida deve passar na validação.");
        Assert.Empty(collection: result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ValidateAsync_WithInvalidTitle_ReturnsError(string? title)
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Title = title!,
            Description = "Descrição válida"
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: result.IsValid, userMessage: "Título vazio deve reprovar na validação.");
        Assert.Contains(collection: result.Errors, filter: static e => e.PropertyName == nameof(CreateCategoryRequest.Title));
    }

    [Fact]
    public async Task ValidateAsync_WithTitleTooShort_ReturnsError()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Title = "AB",
            Description = "Descrição válida"
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: result.IsValid);
        Assert.Contains(collection: result.Errors, filter: static e => e.ErrorMessage == "O título deve conter entre 3 e 80 caracteres.");
    }

    [Fact]
    public async Task ValidateAsync_WithTitleTooLong_ReturnsError()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Title = new string(c: 'A', count: 81),
            Description = "Descrição válida"
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: result.IsValid);
        Assert.Contains(collection: result.Errors, filter: static e => e.ErrorMessage == "O título deve conter entre 3 e 80 caracteres.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ValidateAsync_WithInvalidDescription_ReturnsError(string? description)
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Title = "Título válido",
            Description = description!
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: result.IsValid, userMessage: "Descrição vazia deve reprovar na validação.");
        Assert.Contains(collection: result.Errors, filter: static e => e.PropertyName == nameof(CreateCategoryRequest.Description));
    }

    [Fact]
    public async Task ValidateAsync_WithDescriptionTooShort_ReturnsError()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Title = "Título válido",
            Description = "AB"
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: result.IsValid);
        Assert.Contains(collection: result.Errors, filter: static e => e.ErrorMessage == "A descrição deve conter entre 3 e 180 caracteres.");
    }

    [Fact]
    public async Task ValidateAsync_WithDescriptionTooLong_ReturnsError()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Title = "Título válido",
            Description = new string(c: 'A', count: 181)
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: result.IsValid);
        Assert.Contains(collection: result.Errors, filter: static e => e.ErrorMessage == "A descrição deve conter entre 3 e 180 caracteres.");
    }
}
