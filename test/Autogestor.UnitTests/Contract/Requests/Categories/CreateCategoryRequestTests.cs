using System.ComponentModel.DataAnnotations;
using Autogestor.Contract.Requests.Categories;

namespace Autogestor.UnitTests.Contract.Requests.Categories;

public sealed class CreateCategoryRequestTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(instance: model, serviceProvider: null, items: null);
        Validator.TryValidateObject(instance: model, validationContext: validationContext, validationResults: validationResults, validateAllProperties: true);
        return validationResults;
    }

    [Fact]
    public void CreateCategoryRequest_WithValidData_PassesValidation()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Title = "Investimentos",
            Description = "Categoria para despesas de investimento"
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Empty(collection: errors);
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901")]
    public void CreateCategoryRequest_WithInvalidTitleLength_FailsValidation(string invalidTitle)
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Title = invalidTitle,
            Description = "Descrição válida da categoria"
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Contains(collection: errors, filter: e => e.MemberNames.Contains(value: nameof(CreateCategoryRequest.Title)));
    }

    [Theory]
    [InlineData("ab")]
    public void CreateCategoryRequest_WithShortDescription_FailsValidation(string shortDescription)
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Title = "Título Válido",
            Description = shortDescription
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Contains(collection: errors, filter: e => e.MemberNames.Contains(value: nameof(CreateCategoryRequest.Description)));
    }

    [Fact]
    public void CreateCategoryRequest_WithDescriptionExceeding180Chars_FailsValidation()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Title = "Título Válido",
            Description = new string(c: 'A', count: 181)
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Contains(collection: errors, filter: e => e.MemberNames.Contains(value: nameof(CreateCategoryRequest.Description)));
    }
}
