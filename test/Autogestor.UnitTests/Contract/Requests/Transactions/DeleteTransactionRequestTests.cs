using System.ComponentModel.DataAnnotations;
using Autogestor.Contract.Requests.Transactions;

namespace Autogestor.UnitTests.Contract.Requests.Transactions;

public class DeleteTransactionRequestTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(instance: model, serviceProvider: null, items: null);
        Validator.TryValidateObject(instance: model, validationContext: validationContext, validationResults: validationResults, validateAllProperties: true);
        return validationResults;
    }

    [Fact]
    public void DeleteTransactionRequest_WithValidData_PassesValidation()
    {
        // Arrange
        var transactionId = Guid.NewGuid();

        // Act
        var request = new DeleteTransactionRequest
        {
            Id = transactionId
        };

        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Empty(collection: errors);
        Assert.Equal(expected: transactionId, actual: request.Id);
    }
}
