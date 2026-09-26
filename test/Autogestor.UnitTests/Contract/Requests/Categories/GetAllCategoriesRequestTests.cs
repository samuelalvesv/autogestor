using System.ComponentModel.DataAnnotations;
using Autogestor.Contract;
using Autogestor.Contract.Requests.Categories;

namespace Autogestor.UnitTests.Contract.Requests.Categories;

public sealed class GetAllCategoriesRequestTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(instance: model, serviceProvider: null, items: null);
        Validator.TryValidateObject(instance: model, validationContext: validationContext, validationResults: validationResults, validateAllProperties: true);
        return validationResults;
    }

    [Fact]
    public void GetAllCategoriesRequest_WithValidPagination_PassesValidation()
    {
        // Arrange
        var request = new GetAllCategoriesRequest
        {
            Cursor = Guid.NewGuid(),
            PageSize = ContractDefaults.DefaultPageSize
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Empty(collection: errors);
    }

    [Fact]
    public void GetAllCategoriesRequest_WithNullCursor_PassesValidation()
    {
        // Arrange
        var request = new GetAllCategoriesRequest
        {
            Cursor = null,
            PageSize = ContractDefaults.DefaultPageSize
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Empty(collection: errors);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(51)]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetAllCategoriesRequest_WithInvalidPagination_FailsValidation(int pageSize)
    {
        // Arrange
        var request = new GetAllCategoriesRequest
        {
            PageSize = pageSize
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.NotEmpty(collection: errors);
    }
}
