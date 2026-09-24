using Autogestor.Application.UseCases.Transactions.Reads.GetTransactionById;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.UnitTests.Common.Fakes;

namespace Autogestor.UnitTests.Application.UseCases.Transactions.Reads;

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

        await repository.AddAsync(transaction: transaction, cancellationToken: TestContext.Current.CancellationToken);

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
