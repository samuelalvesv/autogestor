using Autogestor.Application.UseCases.Transactions.Queries.GetAllTransactions;
using Autogestor.Application.Validators.Transactions;
using Autogestor.Contract.Enums;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.UnitTests.Common.Fakes;
using DomainTransactionType = Autogestor.Domain.Enums.ETransactionType;

namespace Autogestor.UnitTests.Application.UseCases.Transactions.Queries;

public sealed class GetAllTransactionsUseCaseTests
{
    private readonly GetAllTransactionsRequestValidator _validator = new();

    [Fact]
    public async Task ExecuteAsync_WhenTransactionsExist_ReturnsPagedResponseWithMappedData()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(
            transactionRepository: repository,
            validator: _validator);

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
            Cursor = null,
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
        Assert.False(condition: response.HasNextPage, userMessage: "Com apenas 2 itens e pageSize 10, não deve haver próxima página.");
        Assert.Null(@object: response.NextCursor);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidPageSize_ThrowsDomainValidationException()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(
            transactionRepository: repository,
            validator: _validator);

        var request = new GetAllTransactionsRequest
        {
            Cursor = null,
            PageSize = 0
        };

        // Act & Assert
        DomainValidationException exception = await Assert.ThrowsAsync<DomainValidationException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Contains(expectedSubstring: "O tamanho da página deve estar entre", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTransactionsExist_MapsAllFieldsCorrectly()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(
            transactionRepository: repository,
            validator: _validator);

        var categoryId = Guid.NewGuid();
        var transaction = Transaction.Create(
            title: "Depósito Inicial",
            type: DomainTransactionType.Deposit,
            amount: 1500.00m,
            categoryId: categoryId);
        repository.Add(transaction: transaction);

        var request = new GetAllTransactionsRequest
        {
            Cursor = null,
            PageSize = 10
        };

        // Act
        PagedResponse<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(collection: response.Data);
        TransactionResponse item = response.Data[index: 0];
        Assert.Equal(expected: transaction.Id, actual: item.Id);
        Assert.Equal(expected: transaction.Title, actual: item.Title);
        Assert.Equal(expected: ETransactionType.Deposit, actual: item.Type);
        Assert.Equal(expected: transaction.Amount, actual: item.Amount);
        Assert.Equal(expected: transaction.CategoryId, actual: item.CategoryId);
        Assert.True(condition: item.Active, userMessage: "A transação retornada deve estar ativa.");
        Assert.Equal(expected: transaction.TenantId, actual: item.TenantId);
        Assert.Equal(expected: transaction.CreatedBy, actual: item.CreatedBy);
        Assert.Equal(expected: transaction.CreatedAt, actual: item.CreatedAt);
        Assert.Equal(expected: transaction.UpdatedBy, actual: item.UpdatedBy);
        Assert.Equal(expected: transaction.UpdatedAt, actual: item.UpdatedAt);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoTransactionsExist_ReturnsEmptyPagedResponse()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(
            transactionRepository: repository,
            validator: _validator);

        var request = new GetAllTransactionsRequest
        {
            Cursor = null,
            PageSize = 10
        };

        // Act
        PagedResponse<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Empty(collection: response.Data);
        Assert.False(condition: response.HasNextPage, userMessage: "Lista vazia não deve indicar próxima página.");
        Assert.Null(@object: response.NextCursor);
    }

    [Fact]
    public async Task ExecuteAsync_WhenMoreItemsThanPageSize_ReturnsHasNextPageTrue()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(
            transactionRepository: repository,
            validator: _validator);

        var categoryId = Guid.NewGuid();
        for (int i = 0; i < 15; i++)
        {
            repository.Add(transaction: Transaction.Create(
                title: $"Tx {i}",
                type: DomainTransactionType.Deposit,
                amount: 100.00m,
                categoryId: categoryId));
        }

        var request = new GetAllTransactionsRequest
        {
            Cursor = null,
            PageSize = 10
        };

        // Act
        PagedResponse<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expected: 10, actual: response.Data.Count);
        Assert.True(condition: response.HasNextPage, userMessage: "Com 15 itens e pageSize 10, deve haver próxima página.");
        Assert.NotNull(@object: response.NextCursor);
    }

    [Fact]
    public async Task ExecuteAsync_WhenExactlyPageSizeItems_ReturnsHasNextPageFalse()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(
            transactionRepository: repository,
            validator: _validator);

        var categoryId = Guid.NewGuid();
        for (int i = 0; i < 10; i++)
        {
            repository.Add(transaction: Transaction.Create(
                title: $"Tx {i}",
                type: DomainTransactionType.Deposit,
                amount: 100.00m,
                categoryId: categoryId));
        }

        var request = new GetAllTransactionsRequest
        {
            Cursor = null,
            PageSize = 10
        };

        // Act
        PagedResponse<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expected: 10, actual: response.Data.Count);
        Assert.False(condition: response.HasNextPage, userMessage: "Com exatamente 10 itens e pageSize 10, não deve haver próxima página.");
        Assert.Null(@object: response.NextCursor);
    }

    [Fact]
    public async Task ExecuteAsync_WithCursor_PropagatesCursorToRepository()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(
            transactionRepository: repository,
            validator: _validator);
        var cursor = Guid.NewGuid();

        var request = new GetAllTransactionsRequest
        {
            Cursor = cursor,
            PageSize = 10
        };

        // Act
        await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expected: cursor, actual: repository.LastPagedCursor);
        Assert.Equal(expected: 10, actual: repository.LastPagedPageSize);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullCursor_PropagatesNullCursorToRepository()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(
            transactionRepository: repository,
            validator: _validator);

        var request = new GetAllTransactionsRequest
        {
            Cursor = null,
            PageSize = 10
        };

        // Act
        await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(@object: repository.LastPagedCursor);
        Assert.Equal(expected: 10, actual: repository.LastPagedPageSize);
    }

    [Fact]
    public async Task ExecuteAsync_PropagatesCancellationToken()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(
            transactionRepository: repository,
            validator: _validator);

        var request = new GetAllTransactionsRequest
        {
            Cursor = null,
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
    public async Task ExecuteAsync_NextCursor_IsLastItemIdOfReturnedPage()
    {
        // Arrange
        var repository = new TransactionRepositoryFake();
        var useCase = new GetAllTransactionsUseCase(
            transactionRepository: repository,
            validator: _validator);

        var categoryId = Guid.NewGuid();
        for (int i = 0; i < 15; i++)
        {
            repository.Add(transaction: Transaction.Create(
                title: $"Tx {i}",
                type: DomainTransactionType.Deposit,
                amount: 100.00m,
                categoryId: categoryId));
        }

        var request = new GetAllTransactionsRequest
        {
            Cursor = null,
            PageSize = 10
        };

        // Act
        PagedResponse<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.True(condition: response.HasNextPage, userMessage: "Com 15 itens e pageSize 10, deve haver próxima página.");
        Assert.NotNull(@object: response.NextCursor);
        Assert.Equal(expected: response.Data[^1].Id, actual: response.NextCursor);
    }
}
