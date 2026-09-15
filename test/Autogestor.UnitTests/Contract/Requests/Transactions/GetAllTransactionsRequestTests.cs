using System.ComponentModel.DataAnnotations;
using Autogestor.Contract.Requests.Transactions;

namespace Autogestor.UnitTests.Contract.Requests.Transactions;

public sealed class GetAllTransactionsRequestTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(instance: model, serviceProvider: null, items: null);
        Validator.TryValidateObject(instance: model, validationContext: validationContext, validationResults: validationResults, validateAllProperties: true);
        return validationResults;
    }

    [Fact]
    public void GetAllTransactionsRequest_WithValidPagination_PassesValidation()
    {
        // Arrange
        var request = new GetAllTransactionsRequest
        {
            PageNumber = 1,
            PageSize = 25
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Empty(collection: errors);
    }

    [Theory]
    [InlineData(0, 25)]
    [InlineData(-1, 25)]
    [InlineData(1, 0)]
    [InlineData(1, -5)]
    [InlineData(1, 1001)]
    public void GetAllTransactionsRequest_WithInvalidPagination_FailsValidation(int pageNumber, int pageSize)
    {
        // Arrange
        var request = new GetAllTransactionsRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.NotEmpty(collection: errors);
    }
}
