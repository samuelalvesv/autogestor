using Autogestor.Application.UseCases.Transactions.Queries.GetTransactionById;
using Autogestor.Contract.Enums;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.UnitTests.Common.Fakes;

namespace Autogestor.UnitTests.Application.UseCases.Transactions.Queries;

public sealed class GetTransactionByIdUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenTransactionExists_ReturnsTransactionResponse()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetTransactionByIdUseCase(transactionRepository: repository);

        var transaction = Transaction.Create(
            title: "Salário Mensal",
            type: Autogestor.Domain.Enums.ETransactionType.Deposit,
            amount: 5000.00m,
            categoryId: Guid.NewGuid());

        repository.Add(transaction: transaction);

        var request = new GetTransactionByIdRequest
        {
            Id = transaction.Id
        };

        // Act
        TransactionResponse response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

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
        Assert.True(condition: repository.LastGetByIdAsNoTracking.GetValueOrDefault(), userMessage: "A consulta deve ser executada com asNoTracking habilitado.");
    }

    [Fact]
    public async Task ExecuteAsync_WhenTransactionNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetTransactionByIdUseCase(transactionRepository: repository);

        var request = new GetTransactionByIdRequest
        {
            Id = Guid.NewGuid()
        };

        // Act & Assert
        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(expected: "Transação não encontrada.", actual: exception.Message);
        Assert.True(condition: repository.LastGetByIdAsNoTracking.GetValueOrDefault(), userMessage: "A consulta deve ser executada com asNoTracking habilitado mesmo quando a entidade não for encontrada.");
    }

    [Fact]
    public async Task ExecuteAsync_WhenTransactionHasBeenUpdated_ReturnsTransactionResponseWithUpdatedAuditFields()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetTransactionByIdUseCase(transactionRepository: repository);

        var transaction = Transaction.Create(
            title: "Consultoria Mensal",
            type: Autogestor.Domain.Enums.ETransactionType.Deposit,
            amount: 7500.00m,
            categoryId: Guid.NewGuid());
        repository.Add(transaction: transaction);

        var updatedBy = Guid.NewGuid();
        DateTime updatedAt = DateTime.UtcNow;
        EntityPersistenceHelper.SetAuditUpdateFields(
            entity: transaction,
            updatedBy: updatedBy,
            updatedAt: updatedAt);

        var request = new GetTransactionByIdRequest
        {
            Id = transaction.Id
        };

        // Act
        TransactionResponse response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Equal(expected: transaction.Id, actual: response.Id);
        Assert.Equal(expected: updatedBy, actual: response.UpdatedBy);
        Assert.Equal(expected: updatedAt, actual: response.UpdatedAt);
        Assert.True(condition: repository.LastGetByIdAsNoTracking.GetValueOrDefault(), userMessage: "A consulta deve ser executada com asNoTracking habilitado.");
    }

    [Fact]
    public async Task ExecuteAsync_PropagatesCancellationToken()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetTransactionByIdUseCase(transactionRepository: repository);

        var request = new GetTransactionByIdRequest
        {
            Id = Guid.NewGuid()
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            testCode: () => useCase.ExecuteAsync(request: request, cancellationToken: token));

        Assert.Equal(expected: token, actual: repository.PassedCancellationToken);
    }
}
