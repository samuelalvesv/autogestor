using Autogestor.Api.Services;
using Autogestor.Application.UseCases.Categories.Commands.CreateCategory;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;

namespace Autogestor.UnitTests.Api.Services;

public class CategoryServiceTests
{
    private sealed class CreateCategoryUseCaseFake : ICreateCategoryUseCase
    {
        public CreateCategoryRequest? ReceivedRequest { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }
        public Response<CategoryResponse> ResponseToReturn { get; set; } = null!;

        public Task<Response<CategoryResponse>> ExecuteAsync(
            CreateCategoryRequest request,
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
        var useCaseFake = new CreateCategoryUseCaseFake();
        var service = new CategoryService(createCategoryUseCase: useCaseFake);

        var request = new CreateCategoryRequest
        {
            Title = "Educação",
            Description = "Cursos e livros",
            UserId = Guid.NewGuid()
        };

        var expectedResponse = new Response<CategoryResponse>
        {
            Data = new CategoryResponse
            {
                Id = Guid.NewGuid(),
                Active = true,
                CreatedBy = request.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedBy = null,
                UpdatedAt = null,
                Title = request.Title,
                Description = request.Description,
                UserId = request.UserId
            },
            Message = "Categoria criada com sucesso."
        };

        useCaseFake.ResponseToReturn = expectedResponse;

        // Act
        Response<CategoryResponse> response = await service.CreateAsync(request: request);

        // Assert
        Assert.Same(expected: expectedResponse, actual: response);
        Assert.Same(expected: request, actual: useCaseFake.ReceivedRequest);
    }

    [Fact]
    public async Task CreateAsync_PropagatesCancellationToken()
    {
        // Arrange
        var useCaseFake = new CreateCategoryUseCaseFake();
        var service = new CategoryService(createCategoryUseCase: useCaseFake);

        var request = new CreateCategoryRequest
        {
            Title = "Saúde",
            Description = "Farmácia e consultas",
            UserId = Guid.NewGuid()
        };

        useCaseFake.ResponseToReturn = new Response<CategoryResponse>
        {
            Data = null,
            Message = "Sucesso"
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await service.CreateAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: useCaseFake.ReceivedCancellationToken);
    }

    [Fact]
    public async Task PendingMethods_ReturnPendingImplementationResponse()
    {
        // Arrange
        var useCaseFake = new CreateCategoryUseCaseFake();
        var service = new CategoryService(createCategoryUseCase: useCaseFake);

        var userId = Guid.NewGuid();

        // Act
        Response<DeleteResponse> deleteResponse = await service.DeleteAsync(
            request: new DeleteCategoryRequest
            {
                Id = Guid.NewGuid(),
                UserId = userId
            });

        PagedResponse<CategoryResponse> getAllResponse = await service.GetAllAsync(
            request: new GetAllCategoriesRequest
            {
                PageNumber = 1,
                PageSize = 10,
                UserId = userId
            });

        Response<CategoryResponse> getByIdResponse = await service.GetByIdAsync(
            request: new GetCategoryByIdRequest
            {
                Id = Guid.NewGuid(),
                UserId = userId
            });

        Response<CategoryResponse> updateResponse = await service.UpdateAsync(
            request: new UpdateCategoryRequest
            {
                Id = Guid.NewGuid(),
                Title = "Educação",
                Description = "Livros",
                UserId = userId
            });

        // Assert
        Assert.Equal(expected: "Implementação pendente.", actual: deleteResponse.Message);
        Assert.Equal(expected: "Implementação pendente.", actual: getAllResponse.Message);
        Assert.Equal(expected: "Implementação pendente.", actual: getByIdResponse.Message);
        Assert.Equal(expected: "Implementação pendente.", actual: updateResponse.Message);
    }
}
