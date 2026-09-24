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
        public Response<TransactionResponse> ResponseToReturn { get; set; } = new()
        {
            Data = null,
            Message = string.Empty
        };

        public Task<Response<TransactionResponse>> ExecuteAsync(
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
        public Response<TransactionResponse> ResponseToReturn { get; set; } = new()
        {
            Data = null,
            Message = string.Empty
        };

        public Task<Response<TransactionResponse>> ExecuteAsync(
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
        public Response<DeleteResponse> ResponseToReturn { get; set; } = new()
        {
            Data = null,
            Message = string.Empty
        };

        public Task<Response<DeleteResponse>> ExecuteAsync(
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
        public Response<TransactionResponse> ResponseToReturn { get; set; } = new()
        {
            Data = null,
            Message = string.Empty
        };

        public Task<Response<TransactionResponse>> ExecuteAsync(
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
            Data = null,
            Message = string.Empty,
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
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

        var expectedResponse = new Response<TransactionResponse>
        {
            Data = new TransactionResponse
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
            },
            Message = "Transação criada com sucesso."
        };

        _createUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        Response<TransactionResponse> response = await _service.CreateAsync(
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

        _createUseCaseFake.ResponseToReturn = new Response<TransactionResponse>
        {
            Data = null,
            Message = "Sucesso"
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

        var expectedResponse = new Response<TransactionResponse>
        {
            Data = new TransactionResponse
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
            },
            Message = "Transação encontrada com sucesso."
        };

        _getByIdUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        Response<TransactionResponse> response = await _service.GetByIdAsync(
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

        _getByIdUseCaseFake.ResponseToReturn = new Response<TransactionResponse>
        {
            Data = null,
            Message = "Sucesso"
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

        var expectedResponse = new Response<TransactionResponse>
        {
            Data = new TransactionResponse
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
            },
            Message = "Transação atualizada com sucesso."
        };

        _updateUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        Response<TransactionResponse> response = await _service.UpdateAsync(
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

        _updateUseCaseFake.ResponseToReturn = new Response<TransactionResponse>
        {
            Data = null,
            Message = "Sucesso"
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

        var expectedResponse = new Response<DeleteResponse>
        {
            Data = new DeleteResponse
            {
                Id = request.Id
            },
            Message = "Transação excluída com sucesso."
        };

        _deleteUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        Response<DeleteResponse> response = await _service.DeleteAsync(
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

        _deleteUseCaseFake.ResponseToReturn = new Response<DeleteResponse>
        {
            Data = null,
            Message = "Sucesso"
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
            PageNumber = 1,
            PageSize = 10
        };

        var expectedResponse = new PagedResponse<TransactionResponse>
        {
            Data = [],
            Message = "Transações encontradas com sucesso.",
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
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
            PageNumber = 1,
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
