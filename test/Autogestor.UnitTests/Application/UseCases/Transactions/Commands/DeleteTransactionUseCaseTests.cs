using Autogestor.Application.UseCases.Transactions.Commands.DeleteTransaction;
using Autogestor.Application.Validators.Transactions;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Enums;
using Autogestor.Domain.Exceptions;
using Autogestor.UnitTests.Common.Fakes;

namespace Autogestor.UnitTests.Application.UseCases.Transactions.Commands;

public sealed class DeleteTransactionUseCaseTests
{
    private readonly DeleteTransactionRequestValidator _validator = new();

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ReturnsSuccessResponseAndRemovesTransaction()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new DeleteTransactionUseCase(
            transactionRepository: repository,
            unitOfWork: unitOfWork,
            validator: _validator);

        var existingTransaction = Transaction.Create(
            title: "Supermercado",
            type: ETransactionType.Withdraw,
            amount: 350.75m,
            categoryId: Guid.NewGuid());
        repository.Add(transaction: existingTransaction);

        var request = new DeleteTransactionRequest
        {
            Id = existingTransaction.Id
        };

        // Act
        DeleteResponse response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Equal(expected: existingTransaction.Id, actual: response.Id);

        Assert.Empty(collection: repository.Transactions);
        Assert.Equal(expected: 1, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyId_ThrowsDomainValidationException()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new DeleteTransactionUseCase(
            transactionRepository: repository,
            unitOfWork: unitOfWork,
            validator: _validator);

        var request = new DeleteTransactionRequest
        {
            Id = Guid.Empty
        };

        // Act & Assert
        DomainValidationException exception = await Assert.ThrowsAsync<DomainValidationException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Contains(expectedSubstring: "O identificador da transação é obrigatório.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTransactionNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new DeleteTransactionUseCase(
            transactionRepository: repository,
            unitOfWork: unitOfWork,
            validator: _validator);

        var request = new DeleteTransactionRequest
        {
            Id = Guid.NewGuid()
        };

        // Act & Assert
        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(expected: "Transação não encontrada.", actual: exception.Message);
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
            unitOfWork: unitOfWork,
            validator: _validator);

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

        repository.Add(transaction: targetTransaction);
        repository.Add(transaction: remainingTransaction);

        var request = new DeleteTransactionRequest
        {
            Id = targetTransaction.Id
        };

        // Act
        DeleteResponse response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Equal(expected: targetTransaction.Id, actual: response.Id);

        Assert.Single(collection: repository.Transactions);
        Assert.Equal(expected: remainingTransaction.Id, actual: repository.Transactions[0].Id);
        Assert.Equal(expected: 1, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenExecutedTwice_ThrowsNotFoundOnSecondCall()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new DeleteTransactionUseCase(
            transactionRepository: repository,
            unitOfWork: unitOfWork,
            validator: _validator);

        var existingTransaction = Transaction.Create(
            title: "Assinatura Streaming",
            type: ETransactionType.Withdraw,
            amount: 49.90m,
            categoryId: Guid.NewGuid());
        repository.Add(transaction: existingTransaction);

        var request = new DeleteTransactionRequest
        {
            Id = existingTransaction.Id
        };

        // Act 1
        DeleteResponse firstResponse = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Act 2 & Assert
        NotFoundException secondException = await Assert.ThrowsAsync<NotFoundException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        // Assert
        Assert.NotNull(@object: firstResponse);
        Assert.Equal(expected: existingTransaction.Id, actual: firstResponse.Id);
        Assert.Equal(expected: "Transação não encontrada.", actual: secondException.Message);
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
            unitOfWork: unitOfWork,
            validator: _validator);

        var existingTransaction = Transaction.Create(
            title: "Salário",
            type: ETransactionType.Deposit,
            amount: 5000.00m,
            categoryId: Guid.NewGuid());
        repository.Add(transaction: existingTransaction);

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
