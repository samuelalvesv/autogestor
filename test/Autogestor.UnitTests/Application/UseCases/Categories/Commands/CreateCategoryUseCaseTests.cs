using Autogestor.Application.UseCases.Categories.Commands.CreateCategory;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses.Categories;
using Autogestor.UnitTests.Common.Fakes;

namespace Autogestor.UnitTests.Application.UseCases.Categories.Commands;

public sealed class CreateCategoryUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ReturnsSuccessResponseAndPersistsCategory()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new CreateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork);

        var request = new CreateCategoryRequest
        {
            Title = "Alimentação",
            Description = "Despesas com restaurantes e mercados"
        };

        // Act
        CategoryResponse response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Equal(expected: request.Title, actual: response.Title);
        Assert.Equal(expected: request.Description, actual: response.Description);
        Assert.NotEqual(expected: Guid.Empty, actual: response.Id);
        Assert.True(condition: response.Active, userMessage: "A categoria deve ser criada como ativa por padrão.");
        Assert.NotEqual(expected: Guid.Empty, actual: response.TenantId);
        Assert.Equal(expected: repository.Categories[0].TenantId, actual: response.TenantId);
        Assert.Null(@object: response.UpdatedBy);
        Assert.Null(@object: response.UpdatedAt);

        Assert.Single(collection: repository.Categories);
        Assert.Equal(expected: request.Title, actual: repository.Categories[0].Title);
        Assert.Equal(expected: request.Description, actual: repository.Categories[0].Description);
        Assert.Equal(expected: 1, actual: unitOfWork.CommitCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ExecuteAsync_WithInvalidTitle_ThrowsArgumentException(string? invalidTitle)
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new CreateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork);

        var request = new CreateCategoryRequest
        {
            Title = invalidTitle!,
            Description = "Descrição válida"
        };

        // Act & Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));
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
        var repository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new CreateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork);

        var request = new CreateCategoryRequest
        {
            Title = "Título válido",
            Description = invalidDescription!
        };

        // Act & Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(expected: "description", actual: exception.ParamName);
        Assert.Equal(expected: "A descrição da categoria não pode ser vazia. (Parameter 'description')", actual: exception.Message);
        Assert.Empty(collection: repository.Categories);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithCancellationToken_PropagatesTokenToDependencies()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new CreateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork);

        var request = new CreateCategoryRequest
        {
            Title = "Transporte",
            Description = "Combustível e manutenção"
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await useCase.ExecuteAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: unitOfWork.PassedCancellationToken);
    }
}
