using System.ComponentModel.DataAnnotations;
using Autogestor.Contract.Requests.Categories;

namespace Autogestor.UnitTests.Contract.Requests.Categories;

public class UpdateCategoryRequestTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, serviceProvider: null, items: null);
        Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);
        return validationResults;
    }

    [Fact]
    public void UpdateCategoryRequest_WithValidData_PassesValidation()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest
        {
            Id = categoryId,
            Title = "Alimentação",
            Description = "Gastos com restaurantes e supermercado"
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Empty(collection: errors);
        Assert.Equal(expected: categoryId, actual: request.Id);
        Assert.Equal(expected: "Alimentação", actual: request.Title);
        Assert.Equal(expected: "Gastos com restaurantes e supermercado", actual: request.Description);
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
        Assert.Contains(collection: errors, filter: e => e.MemberNames.Contains(nameof(UpdateCategoryRequest.Title)));
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
        Assert.Contains(collection: errors, filter: e => e.MemberNames.Contains(nameof(UpdateCategoryRequest.Description)));
    }

    [Fact]
    public void UpdateCategoryRequest_WithDescriptionExceeding180Chars_FailsValidation()
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.NewGuid(),
            Title = "Título Válido",
            Description = new string('A', 181)
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Contains(collection: errors, filter: e => e.MemberNames.Contains(nameof(UpdateCategoryRequest.Description)));
    }
}
