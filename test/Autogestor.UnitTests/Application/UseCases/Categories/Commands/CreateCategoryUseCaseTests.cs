using Autogestor.Application.UseCases.Categories.Commands.CreateCategory;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;

namespace Autogestor.UnitTests.Application.UseCases.Categories.Commands;

public class CreateCategoryUseCaseTests
{
    private sealed class FakeCategoryRepository : ICategoryRepository
    {
        public List<Category> Categories { get; } = [];
        public CancellationToken PassedCancellationToken { get; private set; }

        public Task AddAsync(Category category, CancellationToken cancellationToken = default)
        {
            PassedCancellationToken = cancellationToken;
            Categories.Add(item: category);
            return Task.CompletedTask;
        }

        public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(result: Categories.FirstOrDefault(predicate: c => c.Id == id));

        public Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Category>>(result: Categories.AsReadOnly());
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int CommitCount { get; private set; }
        public CancellationToken PassedCancellationToken { get; private set; }

        public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            PassedCancellationToken = cancellationToken;
            CommitCount++;
            return Task.FromResult(result: 1);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ReturnsSuccessResponseAndPersistsCategory()
    {
        // Arrange
        var repository = new FakeCategoryRepository();
        var unitOfWork = new FakeUnitOfWork();
        var useCase = new CreateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork);

        var request = new CreateCategoryRequest
        {
            Title = "Alimentação",
            Description = "Despesas com restaurantes e mercados",
            UserId = Guid.NewGuid()
        };

        // Act
        Response<CategoryResponse> response = await useCase.ExecuteAsync(request: request);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Equal(expected: "Categoria criada com sucesso.", actual: response.Message);
        Assert.Equal(expected: request.Title, actual: response.Data.Title);
        Assert.Equal(expected: request.Description, actual: response.Data.Description);
        Assert.Equal(expected: request.UserId, actual: response.Data.UserId);
        Assert.NotEqual(expected: Guid.Empty, actual: response.Data.Id);
        Assert.True(condition: response.Data.Active);
        Assert.Equal(expected: request.UserId, actual: response.Data.CreatedBy);

        Assert.Single(collection: repository.Categories);
        Assert.Equal(expected: request.Title, actual: repository.Categories[0].Title);
        Assert.Equal(expected: request.Description, actual: repository.Categories[0].Description);
        Assert.Equal(expected: request.UserId, actual: repository.Categories[0].UserId);
        Assert.Equal(expected: 1, actual: unitOfWork.CommitCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ExecuteAsync_WithInvalidTitle_ThrowsArgumentException(string? invalidTitle)
    {
        // Arrange
        var repository = new FakeCategoryRepository();
        var unitOfWork = new FakeUnitOfWork();
        var useCase = new CreateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork);

        var request = new CreateCategoryRequest
        {
            Title = invalidTitle!,
            Description = "Descrição válida",
            UserId = Guid.NewGuid()
        };

        // Act & Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
            testCode: () => useCase.ExecuteAsync(request: request));
        Assert.Equal(expected: "title", actual: exception.ParamName);
        Assert.Equal(expected: "O título da categoria não pode ser vazio. (Parameter 'title')", actual: exception.Message);
        Assert.Empty(collection: repository.Categories);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ExecuteAsync_WithInvalidDescription_ThrowsArgumentException(string? invalidDescription)
    {
        // Arrange
        var repository = new FakeCategoryRepository();
        var unitOfWork = new FakeUnitOfWork();
        var useCase = new CreateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork);

        var request = new CreateCategoryRequest
        {
            Title = "Título válido",
            Description = invalidDescription!,
            UserId = Guid.NewGuid()
        };

        // Act & Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
            testCode: () => useCase.ExecuteAsync(request: request));
        Assert.Equal(expected: "description", actual: exception.ParamName);
        Assert.Equal(expected: "A descrição da categoria não pode ser vazia. (Parameter 'description')", actual: exception.Message);
        Assert.Empty(collection: repository.Categories);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyUserId_ThrowsArgumentException()
    {
        // Arrange
        var repository = new FakeCategoryRepository();
        var unitOfWork = new FakeUnitOfWork();
        var useCase = new CreateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork);

        var request = new CreateCategoryRequest
        {
            Title = "Título válido",
            Description = "Descrição válida",
            UserId = Guid.Empty
        };

        // Act & Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
            testCode: () => useCase.ExecuteAsync(request: request));
        Assert.Equal(expected: "userId", actual: exception.ParamName);
        Assert.Equal(expected: "Usuário inválido. (Parameter 'userId')", actual: exception.Message);
        Assert.Empty(collection: repository.Categories);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithCancellationToken_PropagatesTokenToDependencies()
    {
        // Arrange
        var repository = new FakeCategoryRepository();
        var unitOfWork = new FakeUnitOfWork();
        var useCase = new CreateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork);

        var request = new CreateCategoryRequest
        {
            Title = "Transporte",
            Description = "Combustível e manutenção",
            UserId = Guid.NewGuid()
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

