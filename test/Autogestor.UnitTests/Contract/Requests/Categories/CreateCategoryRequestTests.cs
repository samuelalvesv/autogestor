using System.ComponentModel.DataAnnotations;
using Autogestor.Contract.Requests.Categories;

namespace Autogestor.UnitTests.Contract.Requests.Categories;

public class CreateCategoryRequestTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, serviceProvider: null, items: null);
        Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);
        return validationResults;
    }

    [Fact]
    public void CreateCategoryRequest_WithValidData_PassesValidation()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            UserId = Guid.NewGuid(),
            Title = "Investimentos",
            Description = "Categoria para despesas de investimento"
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(request);

        // Assert
        Assert.Empty(errors);
        Assert.Equal("Investimentos", request.Title);
        Assert.Equal("Categoria para despesas de investimento", request.Description);
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901")]
    public void CreateCategoryRequest_WithInvalidTitleLength_FailsValidation(string invalidTitle)
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            UserId = Guid.NewGuid(),
            Title = invalidTitle,
            Description = "Descrição válida da categoria"
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(request);

        // Assert
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCategoryRequest.Title)));
    }

    [Theory]
    [InlineData("ab")]
    public void CreateCategoryRequest_WithShortDescription_FailsValidation(string shortDescription)
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            UserId = Guid.NewGuid(),
            Title = "Título Válido",
            Description = shortDescription
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(request);

        // Assert
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCategoryRequest.Description)));
    }

    [Fact]
    public void CreateCategoryRequest_WithDescriptionExceeding180Chars_FailsValidation()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            UserId = Guid.NewGuid(),
            Title = "Título Válido",
            Description = new string('A', 181)
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(request);

        // Assert
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCategoryRequest.Description)));
    }
}
