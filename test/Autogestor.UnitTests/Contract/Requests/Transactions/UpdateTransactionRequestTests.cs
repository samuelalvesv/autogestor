using System.ComponentModel.DataAnnotations;
using Autogestor.Contract.Enums;
using Autogestor.Contract.Requests.Transactions;

namespace Autogestor.UnitTests.Contract.Requests.Transactions;

public class UpdateTransactionRequestTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(instance: model, serviceProvider: null, items: null);
        Validator.TryValidateObject(instance: model, validationContext: validationContext, validationResults: validationResults, validateAllProperties: true);
        return validationResults;
    }

    [Fact]
    public void UpdateTransactionRequest_WithValidData_PassesValidation()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var request = new UpdateTransactionRequest
        {
            Id = transactionId,
            Title = "Compra de Peças",
            Type = ETransactionType.Withdraw,
            Amount = 1500.50m,
            CategoryId = categoryId
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Empty(collection: errors);
        Assert.Equal(expected: transactionId, actual: request.Id);
        Assert.Equal(expected: "Compra de Peças", actual: request.Title);
        Assert.Equal(expected: ETransactionType.Withdraw, actual: request.Type);
        Assert.Equal(expected: 1500.50m, actual: request.Amount);
        Assert.Equal(expected: categoryId, actual: request.CategoryId);
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901")]
    public void UpdateTransactionRequest_WithInvalidTitleLength_FailsValidation(string invalidTitle)
    {
        // Arrange
        var request = new UpdateTransactionRequest
        {
            Id = Guid.NewGuid(),
            Title = invalidTitle,
            Type = ETransactionType.Deposit,
            Amount = 100.00m,
            CategoryId = Guid.NewGuid()
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Contains(collection: errors, filter: e => e.MemberNames.Contains(value: nameof(UpdateTransactionRequest.Title)));
    }

    [Theory]
    [InlineData(0.00)]
    [InlineData(-1.00)]
    public void UpdateTransactionRequest_WithZeroOrNegativeAmount_FailsValidation(double invalidAmountDouble)
    {
        // Arrange
        var request = new UpdateTransactionRequest
        {
            Id = Guid.NewGuid(),
            Title = "Título Válido",
            Type = ETransactionType.Withdraw,
            Amount = (decimal)invalidAmountDouble,
            CategoryId = Guid.NewGuid()
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Contains(collection: errors, filter: e => e.MemberNames.Contains(value: nameof(UpdateTransactionRequest.Amount)));
    }

    [Theory]
    [InlineData((ETransactionType)0)]
    [InlineData((ETransactionType)99)]
    [InlineData((ETransactionType)(-1))]
    public void UpdateTransactionRequest_WithInvalidType_FailsValidation(ETransactionType invalidType)
    {
        // Arrange
        var request = new UpdateTransactionRequest
        {
            Id = Guid.NewGuid(),
            Title = "Título Válido",
            Type = invalidType,
            Amount = 100.00m,
            CategoryId = Guid.NewGuid()
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Contains(collection: errors, filter: e => e.MemberNames.Contains(value: nameof(UpdateTransactionRequest.Type)));
    }
}
