using Autogestor.Api.Services;
using Autogestor.Application.UseCases.Categories.Commands.CreateCategory;
using Autogestor.Application.UseCases.Categories.Commands.DeleteCategory;
using Autogestor.Application.UseCases.Categories.Commands.UpdateCategory;
using Autogestor.Application.UseCases.Categories.Reads.GetCategoryById;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;

namespace Autogestor.UnitTests.Api.Services;

public sealed class CategoryServiceTests
{
    private sealed class CreateCategoryUseCaseFake : ICreateCategoryUseCase
    {
        public CreateCategoryRequest? ReceivedRequest { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }
        public Response<CategoryResponse> ResponseToReturn { get; set; } = new()
        {
            Data = null,
            Message = string.Empty
        };

        public Task<Response<CategoryResponse>> ExecuteAsync(
            CreateCategoryRequest request,
            CancellationToken cancellationToken = default)
        {
            ReceivedRequest = request;
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(result: ResponseToReturn);
        }
    }

    private sealed class UpdateCategoryUseCaseFake : IUpdateCategoryUseCase
    {
        public UpdateCategoryRequest? ReceivedRequest { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }
        public Response<CategoryResponse> ResponseToReturn { get; set; } = new()
        {
            Data = null,
            Message = string.Empty
        };

        public Task<Response<CategoryResponse>> ExecuteAsync(
            UpdateCategoryRequest request,
            CancellationToken cancellationToken = default)
        {
            ReceivedRequest = request;
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(result: ResponseToReturn);
        }
    }

    private sealed class DeleteCategoryUseCaseFake : IDeleteCategoryUseCase
    {
        public DeleteCategoryRequest? ReceivedRequest { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }
        public Response<DeleteResponse> ResponseToReturn { get; set; } = new()
        {
            Data = null,
            Message = string.Empty
        };

        public Task<Response<DeleteResponse>> ExecuteAsync(
            DeleteCategoryRequest request,
            CancellationToken cancellationToken = default)
        {
            ReceivedRequest = request;
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(result: ResponseToReturn);
        }
    }

    private sealed class GetCategoryByIdUseCaseFake : IGetCategoryByIdUseCase
    {
        public GetCategoryByIdRequest? ReceivedRequest { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }
        public Response<CategoryResponse> ResponseToReturn { get; set; } = new()
        {
            Data = null,
            Message = string.Empty
        };

        public Task<Response<CategoryResponse>> ExecuteAsync(
            GetCategoryByIdRequest request,
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
        var createUseCaseFake = new CreateCategoryUseCaseFake();
        var deleteUseCaseFake = new DeleteCategoryUseCaseFake();
        var getByIdUseCaseFake = new GetCategoryByIdUseCaseFake();
        var updateUseCaseFake = new UpdateCategoryUseCaseFake();
        var service = new CategoryService(
            createCategoryUseCase: createUseCaseFake,
            deleteCategoryUseCase: deleteUseCaseFake,
            getCategoryByIdUseCase: getByIdUseCaseFake,
            updateCategoryUseCase: updateUseCaseFake);

        var request = new CreateCategoryRequest
        {
            Title = "Educação",
            Description = "Cursos e livros"
        };

        var expectedResponse = new Response<CategoryResponse>
        {
            Data = new CategoryResponse
            {
                Id = Guid.NewGuid(),
                Active = true,
                CreatedBy = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedBy = null,
                UpdatedAt = null,
                Title = request.Title,
                Description = request.Description,
                TenantId = Guid.NewGuid()
            },
            Message = "Categoria criada com sucesso."
        };

        createUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        Response<CategoryResponse> response = await service.CreateAsync(
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
        var createUseCaseFake = new CreateCategoryUseCaseFake();
        var deleteUseCaseFake = new DeleteCategoryUseCaseFake();
        var getByIdUseCaseFake = new GetCategoryByIdUseCaseFake();
        var updateUseCaseFake = new UpdateCategoryUseCaseFake();
        var service = new CategoryService(
            createCategoryUseCase: createUseCaseFake,
            deleteCategoryUseCase: deleteUseCaseFake,
            getCategoryByIdUseCase: getByIdUseCaseFake,
            updateCategoryUseCase: updateUseCaseFake);

        var request = new CreateCategoryRequest
        {
            Title = "Saúde",
            Description = "Farmácia e consultas"
        };

        createUseCaseFake.ResponseToReturn = new Response<CategoryResponse>
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
        var createUseCaseFake = new CreateCategoryUseCaseFake();
        var deleteUseCaseFake = new DeleteCategoryUseCaseFake();
        var getByIdUseCaseFake = new GetCategoryByIdUseCaseFake();
        var updateUseCaseFake = new UpdateCategoryUseCaseFake();
        var service = new CategoryService(
            createCategoryUseCase: createUseCaseFake,
            deleteCategoryUseCase: deleteUseCaseFake,
            getCategoryByIdUseCase: getByIdUseCaseFake,
            updateCategoryUseCase: updateUseCaseFake);

        var request = new GetCategoryByIdRequest
        {
            Id = Guid.NewGuid()
        };

        var expectedResponse = new Response<CategoryResponse>
        {
            Data = new CategoryResponse
            {
                Id = request.Id,
                Active = true,
                CreatedBy = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedBy = null,
                UpdatedAt = null,
                Title = "Alimentação",
                Description = "Restaurantes e compras",
                TenantId = Guid.NewGuid()
            },
            Message = "Categoria encontrada com sucesso."
        };

        getByIdUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        Response<CategoryResponse> response = await service.GetByIdAsync(
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
        var createUseCaseFake = new CreateCategoryUseCaseFake();
        var deleteUseCaseFake = new DeleteCategoryUseCaseFake();
        var getByIdUseCaseFake = new GetCategoryByIdUseCaseFake();
        var updateUseCaseFake = new UpdateCategoryUseCaseFake();
        var service = new CategoryService(
            createCategoryUseCase: createUseCaseFake,
            deleteCategoryUseCase: deleteUseCaseFake,
            getCategoryByIdUseCase: getByIdUseCaseFake,
            updateCategoryUseCase: updateUseCaseFake);

        var request = new GetCategoryByIdRequest
        {
            Id = Guid.NewGuid()
        };

        getByIdUseCaseFake.ResponseToReturn = new Response<CategoryResponse>
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
        var createUseCaseFake = new CreateCategoryUseCaseFake();
        var deleteUseCaseFake = new DeleteCategoryUseCaseFake();
        var getByIdUseCaseFake = new GetCategoryByIdUseCaseFake();
        var updateUseCaseFake = new UpdateCategoryUseCaseFake();
        var service = new CategoryService(
            createCategoryUseCase: createUseCaseFake,
            deleteCategoryUseCase: deleteUseCaseFake,
            getCategoryByIdUseCase: getByIdUseCaseFake,
            updateCategoryUseCase: updateUseCaseFake);

        var request = new UpdateCategoryRequest
        {
            Id = Guid.NewGuid(),
            Title = "Educação Superior",
            Description = "Pós-graduação"
        };

        var expectedResponse = new Response<CategoryResponse>
        {
            Data = new CategoryResponse
            {
                Id = request.Id,
                Active = true,
                CreatedBy = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedBy = Guid.NewGuid(),
                UpdatedAt = DateTime.UtcNow,
                Title = request.Title,
                Description = request.Description,
                TenantId = Guid.NewGuid()
            },
            Message = "Categoria atualizada com sucesso."
        };

        updateUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        Response<CategoryResponse> response = await service.UpdateAsync(
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
        var createUseCaseFake = new CreateCategoryUseCaseFake();
        var deleteUseCaseFake = new DeleteCategoryUseCaseFake();
        var getByIdUseCaseFake = new GetCategoryByIdUseCaseFake();
        var updateUseCaseFake = new UpdateCategoryUseCaseFake();
        var service = new CategoryService(
            createCategoryUseCase: createUseCaseFake,
            deleteCategoryUseCase: deleteUseCaseFake,
            getCategoryByIdUseCase: getByIdUseCaseFake,
            updateCategoryUseCase: updateUseCaseFake);

        var request = new UpdateCategoryRequest
        {
            Id = Guid.NewGuid(),
            Title = "Educação Superior",
            Description = "Pós-graduação"
        };

        updateUseCaseFake.ResponseToReturn = new Response<CategoryResponse>
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
        var createUseCaseFake = new CreateCategoryUseCaseFake();
        var deleteUseCaseFake = new DeleteCategoryUseCaseFake();
        var getByIdUseCaseFake = new GetCategoryByIdUseCaseFake();
        var updateUseCaseFake = new UpdateCategoryUseCaseFake();
        var service = new CategoryService(
            createCategoryUseCase: createUseCaseFake,
            deleteCategoryUseCase: deleteUseCaseFake,
            getCategoryByIdUseCase: getByIdUseCaseFake,
            updateCategoryUseCase: updateUseCaseFake);

        var request = new DeleteCategoryRequest
        {
            Id = Guid.NewGuid()
        };

        var expectedResponse = new Response<DeleteResponse>
        {
            Data = new DeleteResponse
            {
                Id = request.Id
            },
            Message = "Categoria excluída com sucesso."
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
        var createUseCaseFake = new CreateCategoryUseCaseFake();
        var deleteUseCaseFake = new DeleteCategoryUseCaseFake();
        var getByIdUseCaseFake = new GetCategoryByIdUseCaseFake();
        var updateUseCaseFake = new UpdateCategoryUseCaseFake();
        var service = new CategoryService(
            createCategoryUseCase: createUseCaseFake,
            deleteCategoryUseCase: deleteUseCaseFake,
            getCategoryByIdUseCase: getByIdUseCaseFake,
            updateCategoryUseCase: updateUseCaseFake);

        var request = new DeleteCategoryRequest
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
        var createUseCaseFake = new CreateCategoryUseCaseFake();
        var deleteUseCaseFake = new DeleteCategoryUseCaseFake();
        var getByIdUseCaseFake = new GetCategoryByIdUseCaseFake();
        var updateUseCaseFake = new UpdateCategoryUseCaseFake();
        var service = new CategoryService(
            createCategoryUseCase: createUseCaseFake,
            deleteCategoryUseCase: deleteUseCaseFake,
            getCategoryByIdUseCase: getByIdUseCaseFake,
            updateCategoryUseCase: updateUseCaseFake);

        // Act
        PagedResponse<CategoryResponse> getAllResponse = await service.GetAllAsync(
            request: new GetAllCategoriesRequest
            {
                PageNumber = 1,
                PageSize = 10
            },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expected: "Implementação pendente.", actual: getAllResponse.Message);
    }
}
