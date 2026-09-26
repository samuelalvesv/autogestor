using Autogestor.Api.Services;
using Autogestor.Application.UseCases.Categories.Commands.CreateCategory;
using Autogestor.Application.UseCases.Categories.Commands.DeleteCategory;
using Autogestor.Application.UseCases.Categories.Commands.UpdateCategory;
using Autogestor.Application.UseCases.Categories.Queries.GetAllCategories;
using Autogestor.Application.UseCases.Categories.Queries.GetCategoryById;
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
        public CategoryResponse ResponseToReturn { get; set; } = new()
        {
            Id = Guid.NewGuid(),
            Active = true,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = null,
            UpdatedAt = null,
            Title = string.Empty,
            Description = string.Empty,
            TenantId = Guid.NewGuid()
        };

        public Task<CategoryResponse> ExecuteAsync(
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
        public CategoryResponse ResponseToReturn { get; set; } = new()
        {
            Id = Guid.NewGuid(),
            Active = true,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = null,
            UpdatedAt = null,
            Title = string.Empty,
            Description = string.Empty,
            TenantId = Guid.NewGuid()
        };

        public Task<CategoryResponse> ExecuteAsync(
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
        public DeleteResponse ResponseToReturn { get; set; } = new()
        {
            Id = Guid.NewGuid()
        };

        public Task<DeleteResponse> ExecuteAsync(
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
        public CategoryResponse ResponseToReturn { get; set; } = new()
        {
            Id = Guid.NewGuid(),
            Active = true,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = null,
            UpdatedAt = null,
            Title = string.Empty,
            Description = string.Empty,
            TenantId = Guid.NewGuid()
        };

        public Task<CategoryResponse> ExecuteAsync(
            GetCategoryByIdRequest request,
            CancellationToken cancellationToken = default)
        {
            ReceivedRequest = request;
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(result: ResponseToReturn);
        }
    }

    private sealed class GetAllCategoriesUseCaseFake : IGetAllCategoriesUseCase
    {
        public GetAllCategoriesRequest? ReceivedRequest { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }
        public PagedResponse<CategoryResponse> ResponseToReturn { get; set; } = new()
        {
            Data = [],
            HasNextPage = false,
            NextCursor = null
        };

        public Task<PagedResponse<CategoryResponse>> ExecuteAsync(
            GetAllCategoriesRequest request,
            CancellationToken cancellationToken = default)
        {
            ReceivedRequest = request;
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(result: ResponseToReturn);
        }
    }

    private readonly CreateCategoryUseCaseFake _createUseCaseFake;
    private readonly DeleteCategoryUseCaseFake _deleteUseCaseFake;
    private readonly GetAllCategoriesUseCaseFake _getAllUseCaseFake;
    private readonly GetCategoryByIdUseCaseFake _getByIdUseCaseFake;
    private readonly UpdateCategoryUseCaseFake _updateUseCaseFake;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _createUseCaseFake = new CreateCategoryUseCaseFake();
        _deleteUseCaseFake = new DeleteCategoryUseCaseFake();
        _getAllUseCaseFake = new GetAllCategoriesUseCaseFake();
        _getByIdUseCaseFake = new GetCategoryByIdUseCaseFake();
        _updateUseCaseFake = new UpdateCategoryUseCaseFake();
        _service = new CategoryService(
            createCategoryUseCase: _createUseCaseFake,
            deleteCategoryUseCase: _deleteUseCaseFake,
            getAllCategoriesUseCase: _getAllUseCaseFake,
            getCategoryByIdUseCase: _getByIdUseCaseFake,
            updateCategoryUseCase: _updateUseCaseFake);
    }

    [Fact]
    public async Task CreateAsync_DelegatesToUseCase_AndReturnsResponse()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Title = "Educação",
            Description = "Cursos e livros"
        };

        var expectedResponse = new CategoryResponse
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
        };

        _createUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        CategoryResponse response = await _service.CreateAsync(
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
        var request = new CreateCategoryRequest
        {
            Title = "Saúde",
            Description = "Farmácia e consultas"
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
        var request = new GetCategoryByIdRequest
        {
            Id = Guid.NewGuid()
        };

        var expectedResponse = new CategoryResponse
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
        };

        _getByIdUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        CategoryResponse response = await _service.GetByIdAsync(
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
        var request = new GetCategoryByIdRequest
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
        var request = new UpdateCategoryRequest
        {
            Id = Guid.NewGuid(),
            Title = "Educação Superior",
            Description = "Pós-graduação"
        };

        var expectedResponse = new CategoryResponse
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
        };

        _updateUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        CategoryResponse response = await _service.UpdateAsync(
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
        var request = new UpdateCategoryRequest
        {
            Id = Guid.NewGuid(),
            Title = "Educação Superior",
            Description = "Pós-graduação"
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
        var request = new DeleteCategoryRequest
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
        var request = new DeleteCategoryRequest
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
        var request = new GetAllCategoriesRequest
        {
            Cursor = null,
            PageSize = 10
        };

        var expectedResponse = new PagedResponse<CategoryResponse>
        {
            Data = [],
            HasNextPage = false,
            NextCursor = null
        };

        _getAllUseCaseFake.ResponseToReturn = expectedResponse;

        // Act
        PagedResponse<CategoryResponse> response = await _service.GetAllAsync(
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
        var request = new GetAllCategoriesRequest
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
