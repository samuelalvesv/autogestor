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
            PageSize = ContractDefaults.DefaultPageSize
        };

        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Empty(collection: errors);
    }

    [Fact]
    public void PagedRequest_WithCursor_PassesValidation()
    {
        // Arrange & Act
        var request = new TestPagedRequest
        {
            Cursor = Guid.NewGuid(),
            PageSize = ContractDefaults.DefaultPageSize
        };

        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Empty(collection: errors);
    }

    [Fact]
    public void PagedRequest_WithNullCursor_PassesValidation()
    {
        // Arrange & Act
        var request = new TestPagedRequest
        {
            Cursor = null,
            PageSize = ContractDefaults.DefaultPageSize
        };

        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Empty(collection: errors);
        Assert.Null(@object: request.Cursor);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(51)]
    [InlineData(0)]
    [InlineData(-1)]
    public void PagedRequest_WithInvalidPageSize_FailsValidation(int pageSize)
    {
        // Arrange & Act
        var request = new TestPagedRequest
        {
            PageSize = pageSize
        };

        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.NotEmpty(collection: errors);
    }
}
