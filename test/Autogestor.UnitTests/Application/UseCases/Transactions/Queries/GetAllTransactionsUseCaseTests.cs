using Autogestor.Application.UseCases.Transactions.Queries.GetAllTransactions;
using Autogestor.Contract.Enums;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.UnitTests.Common.Fakes;
using DomainTransactionType = Autogestor.Domain.Enums.ETransactionType;

namespace Autogestor.UnitTests.Application.UseCases.Transactions.Queries;

public sealed class GetAllTransactionsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenTransactionsExist_ReturnsSuccessPagedResponseWithMappedData()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(transactionRepository: repository);

        var categoryId = Guid.NewGuid();
        var tx1 = Transaction.Create(
            title: "Depósito Inicial",
            type: DomainTransactionType.Deposit,
            amount: 1500.00m,
            categoryId: categoryId);
        var tx2 = Transaction.Create(
            title: "Pagamento Fornecedor",
            type: DomainTransactionType.Withdraw,
            amount: 450.00m,
            categoryId: categoryId);

        repository.Add(transaction: tx1);
        repository.Add(transaction: tx2);

        var request = new GetAllTransactionsRequest
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        PagedResponse<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Equal(expected: 2, actual: response.Data.Count);
        Assert.Equal(expected: "Transações encontradas com sucesso.", actual: response.Message);
        Assert.Equal(expected: 2, actual: response.TotalCount);
        Assert.Equal(expected: 1, actual: response.PageNumber);
        Assert.Equal(expected: 10, actual: response.PageSize);

        TransactionResponse first = response.Data[index: 0];
        Assert.Equal(expected: tx1.Id, actual: first.Id);
        Assert.Equal(expected: tx1.Title, actual: first.Title);
        Assert.Equal(expected: ETransactionType.Deposit, actual: first.Type);
        Assert.Equal(expected: tx1.Amount, actual: first.Amount);
        Assert.Equal(expected: tx1.CategoryId, actual: first.CategoryId);
        Assert.True(condition: first.Active, userMessage: "A primeira transação retornada deve estar ativa.");
        Assert.Equal(expected: tx1.TenantId, actual: first.TenantId);
        Assert.Equal(expected: tx1.CreatedBy, actual: first.CreatedBy);
        Assert.Equal(expected: tx1.CreatedAt, actual: first.CreatedAt);
        Assert.Equal(expected: tx1.UpdatedBy, actual: first.UpdatedBy);
        Assert.Equal(expected: tx1.UpdatedAt, actual: first.UpdatedAt);

        TransactionResponse second = response.Data[index: 1];
        Assert.Equal(expected: tx2.Id, actual: second.Id);
        Assert.Equal(expected: tx2.Title, actual: second.Title);
        Assert.Equal(expected: ETransactionType.Withdraw, actual: second.Type);
        Assert.Equal(expected: tx2.Amount, actual: second.Amount);
        Assert.Equal(expected: tx2.CategoryId, actual: second.CategoryId);
        Assert.True(condition: second.Active, userMessage: "A segunda transação retornada deve estar ativa.");
        Assert.Equal(expected: tx2.TenantId, actual: second.TenantId);
        Assert.Equal(expected: tx2.CreatedBy, actual: second.CreatedBy);
        Assert.Equal(expected: tx2.CreatedAt, actual: second.CreatedAt);
        Assert.Equal(expected: tx2.UpdatedBy, actual: second.UpdatedBy);
        Assert.Equal(expected: tx2.UpdatedAt, actual: second.UpdatedAt);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTransactionHasBeenUpdated_ReturnsSuccessPagedResponseWithUpdatedAuditFields()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(transactionRepository: repository);

        var transaction = Transaction.Create(
            title: "Despesa Fixa",
            type: DomainTransactionType.Withdraw,
            amount: 300.00m,
            categoryId: Guid.NewGuid());
        repository.Add(transaction: transaction);

        var updatedBy = Guid.NewGuid();
        DateTime updatedAt = DateTime.UtcNow;
        EntityPersistenceHelper.SetAuditUpdateFields(
            entity: transaction,
            updatedBy: updatedBy,
            updatedAt: updatedAt);

        var request = new GetAllTransactionsRequest
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        PagedResponse<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Single(collection: response.Data);
        Assert.Equal(expected: "Transações encontradas com sucesso.", actual: response.Message);

        TransactionResponse item = response.Data[index: 0];
        Assert.Equal(expected: transaction.Id, actual: item.Id);
        Assert.Equal(expected: updatedBy, actual: item.UpdatedBy);
        Assert.Equal(expected: updatedAt, actual: item.UpdatedAt);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoTransactionsExist_ReturnsEmptyPagedResponseWithDataNull()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(transactionRepository: repository);

        var request = new GetAllTransactionsRequest
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        PagedResponse<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Null(@object: response.Data);
        Assert.Equal(expected: "Transações não encontradas.", actual: response.Message);
        Assert.Equal(expected: 0, actual: response.TotalCount);
        Assert.Equal(expected: 1, actual: response.PageNumber);
        Assert.Equal(expected: 10, actual: response.PageSize);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPageExceedsCount_ReturnsEmptyPagedResponseWithDataNull()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(transactionRepository: repository);

        var transaction = Transaction.Create(
            title: "Aluguel",
            type: DomainTransactionType.Withdraw,
            amount: 2500.00m,
            categoryId: Guid.NewGuid());
        repository.Add(transaction: transaction);

        var request = new GetAllTransactionsRequest
        {
            PageNumber = 2,
            PageSize = 10
        };

        // Act
        PagedResponse<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Null(@object: response.Data);
        Assert.Equal(expected: "Transações não encontradas.", actual: response.Message);
        Assert.Equal(expected: 1, actual: response.TotalCount);
        Assert.Equal(expected: 2, actual: response.PageNumber);
        Assert.Equal(expected: 10, actual: response.PageSize);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSubsequentPageHasData_ReturnsSuccessPagedResponseWithPagedSubset()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(transactionRepository: repository);

        var categoryId = Guid.NewGuid();
        var tx1 = Transaction.Create(
            title: "Tx 1",
            type: DomainTransactionType.Deposit,
            amount: 100.00m,
            categoryId: categoryId);
        var tx2 = Transaction.Create(
            title: "Tx 2",
            type: DomainTransactionType.Withdraw,
            amount: 50.00m,
            categoryId: categoryId);
        var tx3 = Transaction.Create(
            title: "Tx 3",
            type: DomainTransactionType.Deposit,
            amount: 75.00m,
            categoryId: categoryId);

        repository.Add(transaction: tx1);
        repository.Add(transaction: tx2);
        repository.Add(transaction: tx3);

        var request = new GetAllTransactionsRequest
        {
            PageNumber = 2,
            PageSize = 2
        };

        // Act
        PagedResponse<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Single(collection: response.Data);
        Assert.Equal(expected: "Transações encontradas com sucesso.", actual: response.Message);
        Assert.Equal(expected: 3, actual: response.TotalCount);
        Assert.Equal(expected: 2, actual: response.PageNumber);
        Assert.Equal(expected: 2, actual: response.PageSize);
        Assert.Equal(expected: tx3.Id, actual: response.Data[index: 0].Id);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPageOffsetExactlyEqualsCount_ReturnsEmptyPagedResponseWithDataNull()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(transactionRepository: repository);

        var categoryId = Guid.NewGuid();
        var tx1 = Transaction.Create(
            title: "Tx 1",
            type: DomainTransactionType.Deposit,
            amount: 100.00m,
            categoryId: categoryId);
        var tx2 = Transaction.Create(
            title: "Tx 2",
            type: DomainTransactionType.Withdraw,
            amount: 50.00m,
            categoryId: categoryId);

        repository.Add(transaction: tx1);
        repository.Add(transaction: tx2);

        var request = new GetAllTransactionsRequest
        {
            PageNumber = 2,
            PageSize = 2
        };

        // Act
        PagedResponse<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Null(@object: response.Data);
        Assert.Equal(expected: "Transações não encontradas.", actual: response.Message);
        Assert.Equal(expected: 2, actual: response.TotalCount);
        Assert.Equal(expected: 2, actual: response.PageNumber);
        Assert.Equal(expected: 2, actual: response.PageSize);
    }

    [Fact]
    public async Task ExecuteAsync_PropagatesPaginationParametersToRepository()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(transactionRepository: repository);

        var request = new GetAllTransactionsRequest
        {
            PageNumber = 3,
            PageSize = 15
        };

        // Act
        PagedResponse<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expected: 30, actual: repository.LastPagedSkip);
        Assert.Equal(expected: 15, actual: repository.LastPagedPageSize);
        Assert.Equal(expected: 3, actual: response.PageNumber);
        Assert.Equal(expected: 15, actual: response.PageSize);
    }

    [Fact]
    public async Task ExecuteAsync_PropagatesCancellationToken()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(transactionRepository: repository);

        var request = new GetAllTransactionsRequest
        {
            PageNumber = 1,
            PageSize = 10
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await useCase.ExecuteAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: repository.PassedCancellationToken);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPageNumberIsMaxInt_ReturnsEmptyPagedResponseWithDataNull()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(transactionRepository: repository);

        var transaction = Transaction.Create(
            title: "Despesa",
            type: DomainTransactionType.Withdraw,
            amount: 50.00m,
            categoryId: Guid.NewGuid());
        repository.Add(transaction: transaction);

        var request = new GetAllTransactionsRequest
        {
            PageNumber = int.MaxValue,
            PageSize = 25
        };

        // Act
        PagedResponse<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Null(@object: response.Data);
        Assert.Equal(expected: "Transações não encontradas.", actual: response.Message);
        Assert.Equal(expected: 1, actual: response.TotalCount);
        Assert.Equal(expected: int.MaxValue, actual: response.PageNumber);
        Assert.Equal(expected: 25, actual: response.PageSize);
        Assert.Equal(expected: int.MaxValue, actual: repository.LastPagedSkip);
    }
}
