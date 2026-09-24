using Autogestor.Api.Services;
using Autogestor.Application.UseCases.Transactions.Commands.CreateTransaction;
using Autogestor.Application.UseCases.Transactions.Commands.DeleteTransaction;
using Autogestor.Application.UseCases.Transactions.Commands.UpdateTransaction;
using Autogestor.Application.UseCases.Transactions.Reads.GetTransactionById;
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

    [Fact]
    public async Task CreateAsync_DelegatesToUseCase_AndReturnsResponse()
    {
        // Arrange
        var createUseCaseFake = new CreateTransactionUseCaseFake();
        var deleteUseCaseFake = new DeleteTransactionUseCaseFake();
        var getByIdUseCaseFake = new GetTransactionByIdUseCaseFake();
        var updateUseCaseFake = new UpdateTransactionUseCaseFake();
        var service = new TransactionService(
            createTransactionUseCase: createUseCaseFake,
            deleteTransactionUseCase: deleteUseCaseFake,
            getTransactionByIdUseCase: getByIdUseCaseFake,
            updateTransactionUseCase: updateUseCaseFake);

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

        createUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        Response<TransactionResponse> response = await service.CreateAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(expected: expectedResponse, actual: response);
        Assert.Same(expected: request, actual: createUseCaseFake.ReceivedRequest);
    }

    [Fact]
    public async Task CreateAsync_PropagatesCancellationToken()
    {
        // Arrange
        var createUseCaseFake = new CreateTransactionUseCaseFake();
        var deleteUseCaseFake = new DeleteTransactionUseCaseFake();
        var getByIdUseCaseFake = new GetTransactionByIdUseCaseFake();
        var updateUseCaseFake = new UpdateTransactionUseCaseFake();
        var service = new TransactionService(
            createTransactionUseCase: createUseCaseFake,
            deleteTransactionUseCase: deleteUseCaseFake,
            getTransactionByIdUseCase: getByIdUseCaseFake,
            updateTransactionUseCase: updateUseCaseFake);

        var request = new CreateTransactionRequest
        {
            Title = "Venda de Serviço",
            Type = ETransactionType.Deposit,
            Amount = 250.00m,
            CategoryId = Guid.NewGuid()
        };

        createUseCaseFake.ResponseToReturn = new Response<TransactionResponse>
        {
            Data = null,
            Message = "Sucesso"
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await service.CreateAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: createUseCaseFake.ReceivedCancellationToken);
    }

    [Fact]
    public async Task GetByIdAsync_DelegatesToUseCase_AndReturnsResponse()
    {
        // Arrange
        var createUseCaseFake = new CreateTransactionUseCaseFake();
        var deleteUseCaseFake = new DeleteTransactionUseCaseFake();
        var getByIdUseCaseFake = new GetTransactionByIdUseCaseFake();
        var updateUseCaseFake = new UpdateTransactionUseCaseFake();
        var service = new TransactionService(
            createTransactionUseCase: createUseCaseFake,
            deleteTransactionUseCase: deleteUseCaseFake,
            getTransactionByIdUseCase: getByIdUseCaseFake,
            updateTransactionUseCase: updateUseCaseFake);

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

        getByIdUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        Response<TransactionResponse> response = await service.GetByIdAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(expected: expectedResponse, actual: response);
        Assert.Same(expected: request, actual: getByIdUseCaseFake.ReceivedRequest);
    }

    [Fact]
    public async Task GetByIdAsync_PropagatesCancellationToken()
    {
        // Arrange
        var createUseCaseFake = new CreateTransactionUseCaseFake();
        var deleteUseCaseFake = new DeleteTransactionUseCaseFake();
        var getByIdUseCaseFake = new GetTransactionByIdUseCaseFake();
        var updateUseCaseFake = new UpdateTransactionUseCaseFake();
        var service = new TransactionService(
            createTransactionUseCase: createUseCaseFake,
            deleteTransactionUseCase: deleteUseCaseFake,
            getTransactionByIdUseCase: getByIdUseCaseFake,
            updateTransactionUseCase: updateUseCaseFake);

        var request = new GetTransactionByIdRequest
        {
            Id = Guid.NewGuid()
        };

        getByIdUseCaseFake.ResponseToReturn = new Response<TransactionResponse>
        {
            Data = null,
            Message = "Sucesso"
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await service.GetByIdAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: getByIdUseCaseFake.ReceivedCancellationToken);
    }

    [Fact]
    public async Task UpdateAsync_DelegatesToUseCase_AndReturnsResponse()
    {
        // Arrange
        var createUseCaseFake = new CreateTransactionUseCaseFake();
        var deleteUseCaseFake = new DeleteTransactionUseCaseFake();
        var getByIdUseCaseFake = new GetTransactionByIdUseCaseFake();
        var updateUseCaseFake = new UpdateTransactionUseCaseFake();
        var service = new TransactionService(
            createTransactionUseCase: createUseCaseFake,
            deleteTransactionUseCase: deleteUseCaseFake,
            getTransactionByIdUseCase: getByIdUseCaseFake,
            updateTransactionUseCase: updateUseCaseFake);

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

        updateUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        Response<TransactionResponse> response = await service.UpdateAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(expected: expectedResponse, actual: response);
        Assert.Same(expected: request, actual: updateUseCaseFake.ReceivedRequest);
    }

    [Fact]
    public async Task UpdateAsync_PropagatesCancellationToken()
    {
        // Arrange
        var createUseCaseFake = new CreateTransactionUseCaseFake();
        var deleteUseCaseFake = new DeleteTransactionUseCaseFake();
        var getByIdUseCaseFake = new GetTransactionByIdUseCaseFake();
        var updateUseCaseFake = new UpdateTransactionUseCaseFake();
        var service = new TransactionService(
            createTransactionUseCase: createUseCaseFake,
            deleteTransactionUseCase: deleteUseCaseFake,
            getTransactionByIdUseCase: getByIdUseCaseFake,
            updateTransactionUseCase: updateUseCaseFake);

        var request = new UpdateTransactionRequest
        {
            Id = Guid.NewGuid(),
            Title = "Venda de Produto Atualizada",
            Type = ETransactionType.Deposit,
            Amount = 1800.00m,
            CategoryId = Guid.NewGuid()
        };

        updateUseCaseFake.ResponseToReturn = new Response<TransactionResponse>
        {
            Data = null,
            Message = "Sucesso"
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await service.UpdateAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: updateUseCaseFake.ReceivedCancellationToken);
    }

    [Fact]
    public async Task DeleteAsync_DelegatesToUseCase_AndReturnsResponse()
    {
        // Arrange
        var createUseCaseFake = new CreateTransactionUseCaseFake();
        var deleteUseCaseFake = new DeleteTransactionUseCaseFake();
        var getByIdUseCaseFake = new GetTransactionByIdUseCaseFake();
        var updateUseCaseFake = new UpdateTransactionUseCaseFake();
        var service = new TransactionService(
            createTransactionUseCase: createUseCaseFake,
            deleteTransactionUseCase: deleteUseCaseFake,
            getTransactionByIdUseCase: getByIdUseCaseFake,
            updateTransactionUseCase: updateUseCaseFake);

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

        deleteUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        Response<DeleteResponse> response = await service.DeleteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(expected: expectedResponse, actual: response);
        Assert.Same(expected: request, actual: deleteUseCaseFake.ReceivedRequest);
    }

    [Fact]
    public async Task DeleteAsync_PropagatesCancellationToken()
    {
        // Arrange
        var createUseCaseFake = new CreateTransactionUseCaseFake();
        var deleteUseCaseFake = new DeleteTransactionUseCaseFake();
        var getByIdUseCaseFake = new GetTransactionByIdUseCaseFake();
        var updateUseCaseFake = new UpdateTransactionUseCaseFake();
        var service = new TransactionService(
            createTransactionUseCase: createUseCaseFake,
            deleteTransactionUseCase: deleteUseCaseFake,
            getTransactionByIdUseCase: getByIdUseCaseFake,
            updateTransactionUseCase: updateUseCaseFake);

        var request = new DeleteTransactionRequest
        {
            Id = Guid.NewGuid()
        };

        deleteUseCaseFake.ResponseToReturn = new Response<DeleteResponse>
        {
            Data = null,
            Message = "Sucesso"
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await service.DeleteAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: deleteUseCaseFake.ReceivedCancellationToken);
    }

    [Fact]
    public async Task PendingMethods_ReturnPendingImplementationResponse()
    {
        // Arrange
        var createUseCaseFake = new CreateTransactionUseCaseFake();
        var deleteUseCaseFake = new DeleteTransactionUseCaseFake();
        var getByIdUseCaseFake = new GetTransactionByIdUseCaseFake();
        var updateUseCaseFake = new UpdateTransactionUseCaseFake();
        var service = new TransactionService(
            createTransactionUseCase: createUseCaseFake,
            deleteTransactionUseCase: deleteUseCaseFake,
            getTransactionByIdUseCase: getByIdUseCaseFake,
            updateTransactionUseCase: updateUseCaseFake);

        // Act
        PagedResponse<TransactionResponse> getAllResponse = await service.GetAllAsync(
            request: new GetAllTransactionsRequest
            {
                PageNumber = 1,
                PageSize = 10
            },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expected: "Implementação pendente.", actual: getAllResponse.Message);
    }
}
