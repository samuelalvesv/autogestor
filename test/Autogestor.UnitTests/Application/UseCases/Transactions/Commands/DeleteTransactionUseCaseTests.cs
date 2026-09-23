using Autogestor.Application.UseCases.Transactions.Commands.DeleteTransaction;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Enums;
using Autogestor.UnitTests.Common.Fakes;

namespace Autogestor.UnitTests.Application.UseCases.Transactions.Commands;

public sealed class DeleteTransactionUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ReturnsSuccessResponseAndRemovesTransaction()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new DeleteTransactionUseCase(
            transactionRepository: repository,
            unitOfWork: unitOfWork);

        var existingTransaction = Transaction.Create(
            title: "Supermercado",
            type: ETransactionType.Withdraw,
            amount: 350.75m,
            categoryId: Guid.NewGuid());
        await repository.AddAsync(
            transaction: existingTransaction,
            cancellationToken: TestContext.Current.CancellationToken);

        var request = new DeleteTransactionRequest
        {
            Id = existingTransaction.Id
        };

        // Act
        Response<DeleteResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Equal(expected: "Transação excluída com sucesso.", actual: response.Message);
        Assert.Equal(expected: existingTransaction.Id, actual: response.Data.Id);

        Assert.Empty(collection: repository.Transactions);
        Assert.Equal(expected: 1, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTransactionNotFound_ReturnsFailureResponse()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new DeleteTransactionUseCase(
            transactionRepository: repository,
            unitOfWork: unitOfWork);

        var request = new DeleteTransactionRequest
        {
            Id = Guid.NewGuid()
        };

        // Act
        Response<DeleteResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Null(@object: response.Data);
        Assert.Equal(expected: "Transação não encontrada.", actual: response.Message);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenMultipleTransactionsExist_RemovesOnlyTargetTransaction()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new DeleteTransactionUseCase(
            transactionRepository: repository,
            unitOfWork: unitOfWork);

        var targetTransaction = Transaction.Create(
            title: "Alvo",
            type: ETransactionType.Withdraw,
            amount: 100.00m,
            categoryId: Guid.NewGuid());
        var remainingTransaction = Transaction.Create(
            title: "Restante",
            type: ETransactionType.Deposit,
            amount: 200.00m,
            categoryId: Guid.NewGuid());

        await repository.AddAsync(
            transaction: targetTransaction,
            cancellationToken: TestContext.Current.CancellationToken);
        await repository.AddAsync(
            transaction: remainingTransaction,
            cancellationToken: TestContext.Current.CancellationToken);

        var request = new DeleteTransactionRequest
        {
            Id = targetTransaction.Id
        };

        // Act
        Response<DeleteResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Equal(expected: "Transação excluída com sucesso.", actual: response.Message);
        Assert.Equal(expected: targetTransaction.Id, actual: response.Data.Id);

        Assert.Single(collection: repository.Transactions);
        Assert.Equal(expected: remainingTransaction.Id, actual: repository.Transactions[0].Id);
        Assert.Equal(expected: 1, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenExecutedTwice_ReturnsNotFoundOnSecondCall()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new DeleteTransactionUseCase(
            transactionRepository: repository,
            unitOfWork: unitOfWork);

        var existingTransaction = Transaction.Create(
            title: "Assinatura Streaming",
            type: ETransactionType.Withdraw,
            amount: 49.90m,
            categoryId: Guid.NewGuid());
        await repository.AddAsync(
            transaction: existingTransaction,
            cancellationToken: TestContext.Current.CancellationToken);

        var request = new DeleteTransactionRequest
        {
            Id = existingTransaction.Id
        };

        // Act 1
        Response<DeleteResponse> firstResponse = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Act 2
        Response<DeleteResponse> secondResponse = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: firstResponse.Data);
        Assert.Equal(expected: "Transação excluída com sucesso.", actual: firstResponse.Message);
        Assert.Null(@object: secondResponse.Data);
        Assert.Equal(expected: "Transação não encontrada.", actual: secondResponse.Message);
        Assert.Equal(expected: 1, actual: unitOfWork.CommitCount);
        Assert.Empty(collection: repository.Transactions);
    }

    [Fact]
    public async Task ExecuteAsync_WithCancellationToken_PropagatesTokenToDependencies()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new DeleteTransactionUseCase(
            transactionRepository: repository,
            unitOfWork: unitOfWork);

        var existingTransaction = Transaction.Create(
            title: "Salário",
            type: ETransactionType.Deposit,
            amount: 5000.00m,
            categoryId: Guid.NewGuid());
        await repository.AddAsync(
            transaction: existingTransaction,
            cancellationToken: TestContext.Current.CancellationToken);

        var request = new DeleteTransactionRequest
        {
            Id = existingTransaction.Id
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await useCase.ExecuteAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: repository.PassedCancellationToken);
        Assert.Equal(expected: token, actual: unitOfWork.PassedCancellationToken);
    }
}
