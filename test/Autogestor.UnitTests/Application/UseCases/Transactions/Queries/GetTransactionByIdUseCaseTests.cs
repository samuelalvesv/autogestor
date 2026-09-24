using Autogestor.Application.UseCases.Transactions.Queries.GetTransactionById;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.UnitTests.Common.Fakes;

namespace Autogestor.UnitTests.Application.UseCases.Transactions.Queries;

public sealed class GetTransactionByIdUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenTransactionExists_ReturnsSuccessResponseWithData()
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
        Response<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Equal(expected: "Transação encontrada com sucesso.", actual: response.Message);
        Assert.Equal(expected: transaction.Id, actual: response.Data.Id);
        Assert.Equal(expected: transaction.Title, actual: response.Data.Title);
        Assert.Equal(expected: Autogestor.Contract.Enums.ETransactionType.Deposit, actual: response.Data.Type);
        Assert.Equal(expected: transaction.Amount, actual: response.Data.Amount);
        Assert.Equal(expected: transaction.CategoryId, actual: response.Data.CategoryId);
        Assert.Equal(expected: transaction.Active, actual: response.Data.Active);
        Assert.Equal(expected: transaction.TenantId, actual: response.Data.TenantId);
        Assert.Equal(expected: transaction.CreatedBy, actual: response.Data.CreatedBy);
        Assert.Equal(expected: transaction.CreatedAt, actual: response.Data.CreatedAt);
        Assert.Equal(expected: transaction.UpdatedBy, actual: response.Data.UpdatedBy);
        Assert.Equal(expected: transaction.UpdatedAt, actual: response.Data.UpdatedAt);
        Assert.True(condition: repository.LastGetByIdAsNoTracking.GetValueOrDefault(), userMessage: "A consulta deve ser executada com asNoTracking habilitado.");
    }

    [Fact]
    public async Task ExecuteAsync_WhenTransactionNotFound_ReturnsNotFoundResponse()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetTransactionByIdUseCase(transactionRepository: repository);

        var request = new GetTransactionByIdRequest
        {
            Id = Guid.NewGuid()
        };

        // Act
        Response<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Null(@object: response.Data);
        Assert.Equal(expected: "Transação não encontrada.", actual: response.Message);
        Assert.True(condition: repository.LastGetByIdAsNoTracking.GetValueOrDefault(), userMessage: "A consulta deve ser executada com asNoTracking habilitado mesmo quando a entidade não for encontrada.");
    }

    [Fact]
    public async Task ExecuteAsync_WhenTransactionHasBeenUpdated_ReturnsSuccessResponseWithUpdatedAuditFields()
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
        Response<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Equal(expected: "Transação encontrada com sucesso.", actual: response.Message);
        Assert.Equal(expected: transaction.Id, actual: response.Data.Id);
        Assert.Equal(expected: updatedBy, actual: response.Data.UpdatedBy);
        Assert.Equal(expected: updatedAt, actual: response.Data.UpdatedAt);
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

        // Act
        await useCase.ExecuteAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: repository.PassedCancellationToken);
    }
}
