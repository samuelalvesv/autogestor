using Autogestor.Contract.Enums;
using Autogestor.Contract.Responses.Transactions;

namespace Autogestor.UnitTests.Contract.Responses.Transactions;

public class TransactionResponseTests
{
    [Fact]
    public void TransactionResponse_WithValidData_SetsPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var updatedBy = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        DateTime createdAt = DateTime.UtcNow;
        DateTime updatedAt = DateTime.UtcNow.AddHours(value: 1);

        // Act
        var response = new TransactionResponse
        {
            Id = id,
            Active = true,
            CreatedBy = createdBy,
            CreatedAt = createdAt,
            UpdatedBy = updatedBy,
            UpdatedAt = updatedAt,
            TenantId = tenantId,
            Title = "Salário",
            Type = ETransactionType.Deposit,
            Amount = 5000.00m,
            CategoryId = categoryId
        };

        // Assert
        Assert.Equal(expected: id, actual: response.Id);
        Assert.True(condition: response.Active, userMessage: "O DTO da transação deve reportar estado ativo.");
        Assert.Equal(expected: createdBy, actual: response.CreatedBy);
        Assert.Equal(expected: createdAt, actual: response.CreatedAt);
        Assert.Equal(expected: updatedBy, actual: response.UpdatedBy);
        Assert.Equal(expected: updatedAt, actual: response.UpdatedAt);
        Assert.Equal(expected: tenantId, actual: response.TenantId);
        Assert.Equal(expected: "Salário", actual: response.Title);
        Assert.Equal(expected: ETransactionType.Deposit, actual: response.Type);
        Assert.Equal(expected: 5000.00m, actual: response.Amount);
        Assert.Equal(expected: categoryId, actual: response.CategoryId);
    }

    [Fact]
    public void TransactionResponse_WithExplicitNullAuditFields_AllowsNulls()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        DateTime createdAt = DateTime.UtcNow;

        // Act
        var response = new TransactionResponse
        {
            Id = id,
            Active = false,
            CreatedBy = createdBy,
            CreatedAt = createdAt,
            UpdatedBy = null,
            UpdatedAt = null,
            TenantId = tenantId,
            Title = "Aluguel",
            Type = ETransactionType.Withdraw,
            Amount = 1200.00m,
            CategoryId = categoryId
        };

        // Assert
        Assert.Equal(expected: id, actual: response.Id);
        Assert.False(condition: response.Active, userMessage: "O DTO da transação deve reportar estado inativo.");
        Assert.Equal(expected: createdBy, actual: response.CreatedBy);
        Assert.Equal(expected: createdAt, actual: response.CreatedAt);
        Assert.Null(@object: response.UpdatedBy);
        Assert.Null(@object: response.UpdatedAt);
        Assert.Equal(expected: tenantId, actual: response.TenantId);
        Assert.Equal(expected: "Aluguel", actual: response.Title);
        Assert.Equal(expected: ETransactionType.Withdraw, actual: response.Type);
        Assert.Equal(expected: 1200.00m, actual: response.Amount);
        Assert.Equal(expected: categoryId, actual: response.CategoryId);
    }
}
