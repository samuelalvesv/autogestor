using Autogestor.Application.Mappers;
using Autogestor.Contract.Enums;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.UnitTests.Common.Fakes;
using DomainTransactionType = Autogestor.Domain.Enums.ETransactionType;

namespace Autogestor.UnitTests.Application.Mappers;

public sealed class TransactionMapperTests
{
    [Fact]
    public void ToResponse_WithValidTransaction_MapsAllFieldsCorrectly()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var transaction = Transaction.Create(
            title: "Salário Mensal",
            type: DomainTransactionType.Deposit,
            amount: 5000.50m,
            categoryId: categoryId);

        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        DateTime createdAt = DateTime.UtcNow;

        EntityPersistenceHelper.SetPersistenceFields(
            entity: transaction,
            userId: userId,
            tenantId: tenantId,
            timestamp: createdAt);

        var updatedBy = Guid.NewGuid();
        DateTime updatedAt = DateTime.UtcNow.AddMinutes(5);
        EntityPersistenceHelper.SetAuditUpdateFields(
            entity: transaction,
            updatedBy: updatedBy,
            updatedAt: updatedAt);

        // Act
        TransactionResponse response = TransactionMapper.ToResponse(transaction: transaction);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Equal(expected: transaction.Id, actual: response.Id);
        Assert.Equal(expected: transaction.Title, actual: response.Title);
        Assert.Equal(expected: ETransactionType.Deposit, actual: response.Type);
        Assert.Equal(expected: transaction.Amount, actual: response.Amount);
        Assert.Equal(expected: transaction.CategoryId, actual: response.CategoryId);
        Assert.Equal(expected: transaction.Active, actual: response.Active);
        Assert.Equal(expected: transaction.TenantId, actual: response.TenantId);
        Assert.Equal(expected: transaction.CreatedBy, actual: response.CreatedBy);
        Assert.Equal(expected: transaction.CreatedAt, actual: response.CreatedAt);
        Assert.Equal(expected: transaction.UpdatedBy, actual: response.UpdatedBy);
        Assert.Equal(expected: transaction.UpdatedAt, actual: response.UpdatedAt);
    }

    [Fact]
    public void ToResponseList_WithMultipleTransactions_MapsAllItems()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var tx1 = Transaction.Create(
            title: "Tx 1",
            type: DomainTransactionType.Deposit,
            amount: 100m,
            categoryId: categoryId);
        var tx2 = Transaction.Create(
            title: "Tx 2",
            type: DomainTransactionType.Withdraw,
            amount: 50m,
            categoryId: categoryId);

        var list = new List<Transaction> { tx1, tx2 };

        // Act
        IReadOnlyList<TransactionResponse> responses = TransactionMapper.ToResponseList(transactions: list);

        // Assert
        Assert.NotNull(@object: responses);
        Assert.Equal(expected: 2, actual: responses.Count);
        Assert.Equal(expected: tx1.Id, actual: responses[0].Id);
        Assert.Equal(expected: ETransactionType.Deposit, actual: responses[0].Type);
        Assert.Equal(expected: tx2.Id, actual: responses[1].Id);
        Assert.Equal(expected: ETransactionType.Withdraw, actual: responses[1].Type);
    }

    [Fact]
    public void ToPagedResponse_WithItemsAndHasNextPageTrue_ReturnsNextCursorAsLastItemId()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var tx1 = Transaction.Create(
            title: "Tx 1",
            type: DomainTransactionType.Deposit,
            amount: 100m,
            categoryId: categoryId);
        var tx2 = Transaction.Create(
            title: "Tx 2",
            type: DomainTransactionType.Withdraw,
            amount: 50m,
            categoryId: categoryId);
        IReadOnlyList<Transaction> transactions = [tx1, tx2];

        // Act
        var response = TransactionMapper.ToPagedResponse(
            transactions: transactions,
            hasNextPage: true);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Equal(expected: 2, actual: response.Data.Count);
        Assert.True(condition: response.HasNextPage);
        Assert.Equal(expected: tx2.Id, actual: response.NextCursor);
    }

    [Fact]
    public void ToPagedResponse_WithItemsAndHasNextPageFalse_ReturnsNullNextCursor()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var tx1 = Transaction.Create(
            title: "Tx 1",
            type: DomainTransactionType.Deposit,
            amount: 100m,
            categoryId: categoryId);
        IReadOnlyList<Transaction> transactions = [tx1];

        // Act
        var response = TransactionMapper.ToPagedResponse(
            transactions: transactions,
            hasNextPage: false);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Single(collection: response.Data);
        Assert.False(condition: response.HasNextPage);
        Assert.Null(@object: response.NextCursor);
    }

    [Fact]
    public void ToPagedResponse_WithEmptyOrNullList_ReturnsEmptyDataAndNullNextCursor()
    {
        // Act - Empty
        var emptyResponse = TransactionMapper.ToPagedResponse(
            transactions: [],
            hasNextPage: true);

        // Assert - Empty
        Assert.Empty(collection: emptyResponse.Data);
        Assert.True(condition: emptyResponse.HasNextPage);
        Assert.Null(@object: emptyResponse.NextCursor);

        // Act - Null
        var nullResponse = TransactionMapper.ToPagedResponse(
            transactions: null,
            hasNextPage: false);

        // Assert - Null
        Assert.Empty(collection: nullResponse.Data);
        Assert.False(condition: nullResponse.HasNextPage);
        Assert.Null(@object: nullResponse.NextCursor);
    }

    [Theory]
    [InlineData(DomainTransactionType.Deposit, ETransactionType.Deposit)]
    [InlineData(DomainTransactionType.Withdraw, ETransactionType.Withdraw)]
    public void ToContract_MapsEnumCorrectly(DomainTransactionType domainType, ETransactionType expectedContractType)
    {
        // Act
        ETransactionType result = TransactionMapper.ToContract(type: domainType);

        // Assert
        Assert.Equal(expected: expectedContractType, actual: result);
    }

    [Theory]
    [InlineData(ETransactionType.Deposit, DomainTransactionType.Deposit)]
    [InlineData(ETransactionType.Withdraw, DomainTransactionType.Withdraw)]
    public void ToDomain_MapsEnumCorrectly(ETransactionType contractType, DomainTransactionType expectedDomainType)
    {
        // Act
        DomainTransactionType result = TransactionMapper.ToDomain(type: contractType);

        // Assert
        Assert.Equal(expected: expectedDomainType, actual: result);
    }
}
