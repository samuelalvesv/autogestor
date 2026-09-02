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
        var userId = Guid.NewGuid();
        var request = new UpdateCategoryRequest
        {
            Id = categoryId,
            UserId = userId,
            Title = "Alimentação",
            Description = "Gastos com restaurantes e supermercado"
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(request);

        // Assert
        Assert.Empty(errors);
        Assert.Equal(categoryId, request.Id);
        Assert.Equal(userId, request.UserId);
        Assert.Equal("Alimentação", request.Title);
        Assert.Equal("Gastos com restaurantes e supermercado", request.Description);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901")]
    public void UpdateCategoryRequest_WithInvalidTitle_FailsValidation(string invalidTitle)
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Title = invalidTitle,
            Description = "Descrição válida da categoria"
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(request);

        // Assert
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCategoryRequest.Title)));
    }

    [Theory]
    [InlineData("a")]
    public void UpdateCategoryRequest_WithShortDescription_FailsValidation(string shortDescription)
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Title = "Título Válido",
            Description = shortDescription
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(request);

        // Assert
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCategoryRequest.Description)));
    }

    [Fact]
    public void UpdateCategoryRequest_WithDescriptionExceeding180Chars_FailsValidation()
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Title = "Título Válido",
            Description = new string('A', 181)
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(request);

        // Assert
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCategoryRequest.Description)));
    }
}
