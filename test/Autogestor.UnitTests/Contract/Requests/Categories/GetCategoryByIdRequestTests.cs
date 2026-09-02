using System.ComponentModel.DataAnnotations;
using Autogestor.Contract.Requests.Categories;

namespace Autogestor.UnitTests.Contract.Requests.Categories;

public class GetCategoryByIdRequestTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, serviceProvider: null, items: null);
        Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);
        return validationResults;
    }

    [Fact]
    public void GetCategoryByIdRequest_WithValidData_PassesValidation()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var request = new GetCategoryByIdRequest
        {
            Id = categoryId,
            UserId = userId
        };

        IList<ValidationResult> errors = ValidateModel(request);

        // Assert
        Assert.Empty(errors);
        Assert.Equal(categoryId, request.Id);
        Assert.Equal(userId, request.UserId);
    }
}
