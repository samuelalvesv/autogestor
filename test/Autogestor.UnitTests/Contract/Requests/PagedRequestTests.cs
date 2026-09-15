using System.ComponentModel.DataAnnotations;
using Autogestor.Contract;
using Autogestor.Contract.Requests;

namespace Autogestor.UnitTests.Contract.Requests;

public sealed class PagedRequestTests
{
    private sealed record TestPagedRequest : PagedRequest;

    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(instance: model, serviceProvider: null, items: null);
        Validator.TryValidateObject(instance: model, validationContext: validationContext, validationResults: validationResults, validateAllProperties: true);
        return validationResults;
    }

    [Fact]
    public void PagedRequest_WithValidValues_PassesValidation()
    {
        // Arrange & Act
        var request = new TestPagedRequest
        {
            PageNumber = ContractDefaults.DefaultPageNumber,
            PageSize = ContractDefaults.DefaultPageSize
        };

        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Empty(collection: errors);
    }

    [Theory]
    [InlineData(0, 25)]
    [InlineData(-1, 25)]
    [InlineData(1, 5)]
    [InlineData(1, 51)]
    public void PagedRequest_WithInvalidValues_FailsValidation(int pageNumber, int pageSize)
    {
        // Arrange & Act
        var request = new TestPagedRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.NotEmpty(collection: errors);
    }
}
