using Autogestor.Application.UseCases.Categories.Commands.UpdateCategory;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.UnitTests.Common.Fakes;

namespace Autogestor.UnitTests.Application.UseCases.Categories.Commands;

public sealed class UpdateCategoryUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ReturnsSuccessResponseAndUpdatesCategory()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new UpdateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork);

        var existingCategory = Category.Create(
            title: "Alimentação",
            description: "Despesas com mercados");
        repository.Add(category: existingCategory);

        var request = new UpdateCategoryRequest
        {
            Id = existingCategory.Id,
            Title = "Alimentação & Bebidas",
            Description = "Despesas com refeições e supermercados"
        };

        // Act
        Response<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Equal(expected: "Categoria atualizada com sucesso.", actual: response.Message);
        Assert.Equal(expected: request.Id, actual: response.Data.Id);
        Assert.Equal(expected: request.Title, actual: response.Data.Title);
        Assert.Equal(expected: request.Description, actual: response.Data.Description);
        Assert.True(condition: response.Data.Active, userMessage: "A categoria deve permanecer ativa após atualização.");
        Assert.Equal(expected: existingCategory.TenantId, actual: response.Data.TenantId);

        Assert.Single(collection: repository.Categories);
        Assert.Equal(expected: request.Title, actual: repository.Categories[0].Title);
        Assert.Equal(expected: request.Description, actual: repository.Categories[0].Description);
        Assert.Equal(expected: 1, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoryNotFound_ReturnsFailureResponse()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new UpdateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork);

        var request = new UpdateCategoryRequest
        {
            Id = Guid.NewGuid(),
            Title = "Nova Categoria",
            Description = "Descrição válida"
        };

        // Act
        Response<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Null(@object: response.Data);
        Assert.Equal(expected: "Categoria não encontrada.", actual: response.Message);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
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
        var useCase = new UpdateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork);

        var existingCategory = Category.Create(
            title: "Transporte",
            description: "Descrição de transporte");
        repository.Add(category: existingCategory);

        var request = new UpdateCategoryRequest
        {
            Id = existingCategory.Id,
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
        var useCase = new UpdateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork);

        var existingCategory = Category.Create(
            title: "Educação",
            description: "Descrição inicial");
        repository.Add(category: existingCategory);

        var request = new UpdateCategoryRequest
        {
            Id = existingCategory.Id,
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
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithCancellationToken_PropagatesTokenToDependencies()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new UpdateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork);

        var existingCategory = Category.Create(
            title: "Saúde",
            description: "Planos de saúde");
        repository.Add(category: existingCategory);

        var request = new UpdateCategoryRequest
        {
            Id = existingCategory.Id,
            Title = "Saúde & Bem-estar",
            Description = "Convênios e medicamentos"
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
