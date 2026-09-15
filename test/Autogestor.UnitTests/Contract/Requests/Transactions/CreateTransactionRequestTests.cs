using System.ComponentModel.DataAnnotations;
using Autogestor.Contract.Enums;
using Autogestor.Contract.Requests.Transactions;

namespace Autogestor.UnitTests.Contract.Requests.Transactions;

public class CreateTransactionRequestTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(instance: model, serviceProvider: null, items: null);
        Validator.TryValidateObject(instance: model, validationContext: validationContext, validationResults: validationResults, validateAllProperties: true);
        return validationResults;
    }

    [Fact]
    public void CreateTransactionRequest_WithValidData_PassesValidation()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new CreateTransactionRequest
        {
            Title = "Venda de Veículo",
            Type = ETransactionType.Deposit,
            Amount = 45000.00m,
            CategoryId = categoryId
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Empty(collection: errors);
        Assert.Equal(expected: "Venda de Veículo", actual: request.Title);
        Assert.Equal(expected: ETransactionType.Deposit, actual: request.Type);
        Assert.Equal(expected: 45000.00m, actual: request.Amount);
        Assert.Equal(expected: categoryId, actual: request.CategoryId);
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901")]
    public void CreateTransactionRequest_WithInvalidTitleLength_FailsValidation(string invalidTitle)
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            Title = invalidTitle,
            Type = ETransactionType.Deposit,
            Amount = 100.00m,
            CategoryId = Guid.NewGuid()
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Contains(collection: errors, filter: e => e.MemberNames.Contains(value: nameof(CreateTransactionRequest.Title)));
    }

    [Theory]
    [InlineData(0.00)]
    [InlineData(-1.00)]
    [InlineData(-50.50)]
    public void CreateTransactionRequest_WithZeroOrNegativeAmount_FailsValidation(double invalidAmountDouble)
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            Title = "Título Válido",
            Type = ETransactionType.Withdraw,
            Amount = (decimal)invalidAmountDouble,
            CategoryId = Guid.NewGuid()
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Contains(collection: errors, filter: e => e.MemberNames.Contains(value: nameof(CreateTransactionRequest.Amount)));
    }

    [Theory]
    [InlineData((ETransactionType)0)]
    [InlineData((ETransactionType)99)]
    [InlineData((ETransactionType)(-1))]
    public void CreateTransactionRequest_WithInvalidType_FailsValidation(ETransactionType invalidType)
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            Title = "Título Válido",
            Type = invalidType,
            Amount = 100.00m,
            CategoryId = Guid.NewGuid()
        };

        // Act
        IList<ValidationResult> errors = ValidateModel(model: request);

        // Assert
        Assert.Contains(collection: errors, filter: e => e.MemberNames.Contains(value: nameof(CreateTransactionRequest.Type)));
    }
}
