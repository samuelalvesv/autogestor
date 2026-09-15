using System.ComponentModel.DataAnnotations;
using Autogestor.Contract.Requests.Transactions;

namespace Autogestor.UnitTests.Contract.Requests.Transactions;

public class GetAllTransactionsRequestTests
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
        Assert.Equal(expected: 1, actual: request.PageNumber);
        Assert.Equal(expected: 25, actual: request.PageSize);
    }
}
