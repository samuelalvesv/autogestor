using System.ComponentModel.DataAnnotations;
using Autogestor.Contract.Requests.Categories;

namespace Autogestor.UnitTests.Contract.Requests.Categories;

public sealed class UpdateCategoryRequestTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(instance: model, serviceProvider: null, items: null);
        Validator.TryValidateObject(instance: model, validationContext: validationContext, validationResults: validationResults, validateAllProperties: true);
        return validationResults;
    }

    [Fact]
    public void UpdateCategoryRequest_WithValidData_PassesValidation()
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.NewGuid(),
            Title = "Alimentação",
            Description = "Gastos com restaurantes e supermercado"
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Empty(collection: errors);
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901")]
    public void UpdateCategoryRequest_WithInvalidTitleLength_FailsValidation(string invalidTitle)
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.NewGuid(),
            Title = invalidTitle,
            Description = "Descrição válida da categoria"
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Contains(collection: errors, filter: e => e.MemberNames.Contains(value: nameof(UpdateCategoryRequest.Title)));
    }

    [Theory]
    [InlineData("ab")]
    public void UpdateCategoryRequest_WithShortDescription_FailsValidation(string shortDescription)
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.NewGuid(),
            Title = "Título Válido",
            Description = shortDescription
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Contains(collection: errors, filter: e => e.MemberNames.Contains(value: nameof(UpdateCategoryRequest.Description)));
    }

    [Fact]
    public void UpdateCategoryRequest_WithDescriptionExceeding180Chars_FailsValidation()
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.NewGuid(),
            Title = "Título Válido",
            Description = new string(c: 'A', count: 181)
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Contains(collection: errors, filter: e => e.MemberNames.Contains(value: nameof(UpdateCategoryRequest.Description)));
    }
}
