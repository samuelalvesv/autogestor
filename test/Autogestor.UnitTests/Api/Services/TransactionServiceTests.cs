using Autogestor.Api.Services;
using Autogestor.Application.UseCases.Transactions.Commands.CreateTransaction;
using Autogestor.Application.UseCases.Transactions.Commands.DeleteTransaction;
using Autogestor.Application.UseCases.Transactions.Commands.UpdateTransaction;
using Autogestor.Application.UseCases.Transactions.Queries.GetAllTransactions;
using Autogestor.Application.UseCases.Transactions.Queries.GetTransactionById;
using Autogestor.Contract.Enums;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;

namespace Autogestor.UnitTests.Api.Services;

public sealed class TransactionServiceTests
{
    private sealed class CreateTransactionUseCaseFake : ICreateTransactionUseCase
    {
        public CreateTransactionRequest? ReceivedRequest { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }
        public TransactionResponse ResponseToReturn { get; set; } = new()
        {
            Id = Guid.NewGuid(),
            Active = true,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = null,
            UpdatedAt = null,
            Title = string.Empty,
            Type = ETransactionType.Deposit,
            Amount = 0m,
            CategoryId = Guid.NewGuid(),
            TenantId = Guid.NewGuid()
        };

        public Task<TransactionResponse> ExecuteAsync(
            CreateTransactionRequest request,
            CancellationToken cancellationToken = default)
        {
            ReceivedRequest = request;
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(result: ResponseToReturn);
        }
    }

    private sealed class UpdateTransactionUseCaseFake : IUpdateTransactionUseCase
    {
        public UpdateTransactionRequest? ReceivedRequest { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }
        public TransactionResponse ResponseToReturn { get; set; } = new()
        {
            Id = Guid.NewGuid(),
            Active = true,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = null,
            UpdatedAt = null,
            Title = string.Empty,
            Type = ETransactionType.Deposit,
            Amount = 0m,
            CategoryId = Guid.NewGuid(),
            TenantId = Guid.NewGuid()
        };

        public Task<TransactionResponse> ExecuteAsync(
            UpdateTransactionRequest request,
            CancellationToken cancellationToken = default)
        {
            ReceivedRequest = request;
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(result: ResponseToReturn);
        }
    }

    private sealed class DeleteTransactionUseCaseFake : IDeleteTransactionUseCase
    {
        public DeleteTransactionRequest? ReceivedRequest { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }
        public DeleteResponse ResponseToReturn { get; set; } = new()
        {
            Id = Guid.NewGuid()
        };

        public Task<DeleteResponse> ExecuteAsync(
            DeleteTransactionRequest request,
            CancellationToken cancellationToken = default)
        {
            ReceivedRequest = request;
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(result: ResponseToReturn);
        }
    }

    private sealed class GetTransactionByIdUseCaseFake : IGetTransactionByIdUseCase
    {
        public GetTransactionByIdRequest? ReceivedRequest { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }
        public TransactionResponse ResponseToReturn { get; set; } = new()
        {
            Id = Guid.NewGuid(),
            Active = true,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = null,
            UpdatedAt = null,
            Title = string.Empty,
            Type = ETransactionType.Deposit,
            Amount = 0m,
            CategoryId = Guid.NewGuid(),
            TenantId = Guid.NewGuid()
        };

        public Task<TransactionResponse> ExecuteAsync(
            GetTransactionByIdRequest request,
            CancellationToken cancellationToken = default)
        {
            ReceivedRequest = request;
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(result: ResponseToReturn);
        }
    }

    private sealed class GetAllTransactionsUseCaseFake : IGetAllTransactionsUseCase
    {
        public GetAllTransactionsRequest? ReceivedRequest { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }
        public PagedResponse<TransactionResponse> ResponseToReturn { get; set; } = new()
        {
            Data = [],
            HasNextPage = false,
            NextCursor = null
        };

        public Task<PagedResponse<TransactionResponse>> ExecuteAsync(
            GetAllTransactionsRequest request,
            CancellationToken cancellationToken = default)
        {
            ReceivedRequest = request;
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(result: ResponseToReturn);
        }
    }

    private readonly CreateTransactionUseCaseFake _createUseCaseFake;
    private readonly DeleteTransactionUseCaseFake _deleteUseCaseFake;
    private readonly GetAllTransactionsUseCaseFake _getAllUseCaseFake;
    private readonly GetTransactionByIdUseCaseFake _getByIdUseCaseFake;
    private readonly UpdateTransactionUseCaseFake _updateUseCaseFake;
    private readonly TransactionService _service;

    public TransactionServiceTests()
    {
        _createUseCaseFake = new CreateTransactionUseCaseFake();
        _deleteUseCaseFake = new DeleteTransactionUseCaseFake();
        _getAllUseCaseFake = new GetAllTransactionsUseCaseFake();
        _getByIdUseCaseFake = new GetTransactionByIdUseCaseFake();
        _updateUseCaseFake = new UpdateTransactionUseCaseFake();
        _service = new TransactionService(
            createTransactionUseCase: _createUseCaseFake,
            deleteTransactionUseCase: _deleteUseCaseFake,
            getAllTransactionsUseCase: _getAllUseCaseFake,
            getTransactionByIdUseCase: _getByIdUseCaseFake,
            updateTransactionUseCase: _updateUseCaseFake);
    }

    [Fact]
    public async Task CreateAsync_DelegatesToUseCase_AndReturnsResponse()
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            Title = "Venda de Produto",
            Type = ETransactionType.Deposit,
            Amount = 1500.00m,
            CategoryId = Guid.NewGuid()
        };

        var expectedResponse = new TransactionResponse
        {
            Id = Guid.NewGuid(),
            Active = true,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = null,
            UpdatedAt = null,
            Title = request.Title,
            Type = request.Type,
            Amount = request.Amount,
            CategoryId = request.CategoryId,
            TenantId = Guid.NewGuid()
        };

        _createUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        TransactionResponse response = await _service.CreateAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(expected: expectedResponse, actual: response);
        Assert.Same(expected: request, actual: _createUseCaseFake.ReceivedRequest);
    }

    [Fact]
    public async Task CreateAsync_PropagatesCancellationToken()
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            Title = "Venda de Serviço",
            Type = ETransactionType.Deposit,
            Amount = 250.00m,
            CategoryId = Guid.NewGuid()
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await _service.CreateAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: _createUseCaseFake.ReceivedCancellationToken);
    }

    [Fact]
    public async Task GetByIdAsync_DelegatesToUseCase_AndReturnsResponse()
    {
        // Arrange
        var request = new GetTransactionByIdRequest
        {
            Id = Guid.NewGuid()
        };

        var expectedResponse = new TransactionResponse
        {
            Id = request.Id,
            Active = true,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = null,
            UpdatedAt = null,
            Title = "Pagamento",
            Type = ETransactionType.Withdraw,
            Amount = 100.00m,
            CategoryId = Guid.NewGuid(),
            TenantId = Guid.NewGuid()
        };

        _getByIdUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        TransactionResponse response = await _service.GetByIdAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(expected: expectedResponse, actual: response);
        Assert.Same(expected: request, actual: _getByIdUseCaseFake.ReceivedRequest);
    }

    [Fact]
    public async Task GetByIdAsync_PropagatesCancellationToken()
    {
        // Arrange
        var request = new GetTransactionByIdRequest
        {
            Id = Guid.NewGuid()
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await _service.GetByIdAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: _getByIdUseCaseFake.ReceivedCancellationToken);
    }

    [Fact]
    public async Task UpdateAsync_DelegatesToUseCase_AndReturnsResponse()
    {
        // Arrange
        var request = new UpdateTransactionRequest
        {
            Id = Guid.NewGuid(),
            Title = "Venda de Produto Atualizada",
            Type = ETransactionType.Deposit,
            Amount = 1800.00m,
            CategoryId = Guid.NewGuid()
        };

        var expectedResponse = new TransactionResponse
        {
            Id = request.Id,
            Active = true,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = Guid.NewGuid(),
            UpdatedAt = DateTime.UtcNow,
            Title = request.Title,
            Type = ETransactionType.Deposit,
            Amount = request.Amount,
            CategoryId = request.CategoryId,
            TenantId = Guid.NewGuid()
        };

        _updateUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        TransactionResponse response = await _service.UpdateAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(expected: expectedResponse, actual: response);
        Assert.Same(expected: request, actual: _updateUseCaseFake.ReceivedRequest);
    }

    [Fact]
    public async Task UpdateAsync_PropagatesCancellationToken()
    {
        // Arrange
        var request = new UpdateTransactionRequest
        {
            Id = Guid.NewGuid(),
            Title = "Venda de Produto Atualizada",
            Type = ETransactionType.Deposit,
            Amount = 1800.00m,
            CategoryId = Guid.NewGuid()
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await _service.UpdateAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: _updateUseCaseFake.ReceivedCancellationToken);
    }

    [Fact]
    public async Task DeleteAsync_DelegatesToUseCase_AndReturnsResponse()
    {
        // Arrange
        var request = new DeleteTransactionRequest
        {
            Id = Guid.NewGuid()
        };

        var expectedResponse = new DeleteResponse
        {
            Id = request.Id
        };

        _deleteUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        DeleteResponse response = await _service.DeleteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(expected: expectedResponse, actual: response);
        Assert.Same(expected: request, actual: _deleteUseCaseFake.ReceivedRequest);
    }

    [Fact]
    public async Task DeleteAsync_PropagatesCancellationToken()
    {
        // Arrange
        var request = new DeleteTransactionRequest
        {
            Id = Guid.NewGuid()
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await _service.DeleteAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: _deleteUseCaseFake.ReceivedCancellationToken);
    }

    [Fact]
    public async Task GetAllAsync_DelegatesToUseCase_AndReturnsResponse()
    {
        // Arrange
        var request = new GetAllTransactionsRequest
        {
            Cursor = null,
            PageSize = 10
        };

        var expectedResponse = new PagedResponse<TransactionResponse>
        {
            Data = [],
            HasNextPage = false,
            NextCursor = null
        };

        _getAllUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        PagedResponse<TransactionResponse> response = await _service.GetAllAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(expected: expectedResponse, actual: response);
        Assert.Same(expected: request, actual: _getAllUseCaseFake.ReceivedRequest);
    }

    [Fact]
    public async Task GetAllAsync_PropagatesCancellationToken()
    {
        // Arrange
        var request = new GetAllTransactionsRequest
        {
            Cursor = null,
            PageSize = 10
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await _service.GetAllAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: _getAllUseCaseFake.ReceivedCancellationToken);
    }
}
