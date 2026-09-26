using Autogestor.Application.UseCases.Categories.Commands.UpdateCategory;
using Autogestor.Application.Validators.Categories;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.UnitTests.Common.Fakes;

namespace Autogestor.UnitTests.Application.UseCases.Categories.Commands;

public sealed class UpdateCategoryUseCaseTests
{
    private readonly UpdateCategoryRequestValidator _validator = new();

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ReturnsSuccessResponseAndUpdatesCategory()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new UpdateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork,
            validator: _validator);

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
        CategoryResponse response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Equal(expected: request.Id, actual: response.Id);
        Assert.Equal(expected: request.Title, actual: response.Title);
        Assert.Equal(expected: request.Description, actual: response.Description);
        Assert.True(condition: response.Active, userMessage: "A categoria deve permanecer ativa após atualização.");
        Assert.Equal(expected: existingCategory.TenantId, actual: response.TenantId);

        Assert.Single(collection: repository.Categories);
        Assert.Equal(expected: request.Title, actual: repository.Categories[0].Title);
        Assert.Equal(expected: request.Description, actual: repository.Categories[0].Description);
        Assert.Equal(expected: 1, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoryNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new UpdateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork,
            validator: _validator);

        var request = new UpdateCategoryRequest
        {
            Id = Guid.NewGuid(),
            Title = "Nova Categoria",
            Description = "Descrição válida"
        };

        // Act & Assert
        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(expected: "Categoria não encontrada.", actual: exception.Message);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyId_ThrowsDomainValidationException()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new UpdateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork,
            validator: _validator);

        var request = new UpdateCategoryRequest
        {
            Id = Guid.Empty,
            Title = "Título válido",
            Description = "Descrição válida"
        };

        // Act & Assert
        DomainValidationException exception = await Assert.ThrowsAsync<DomainValidationException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Contains(expectedSubstring: "O identificador da categoria é obrigatório.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ExecuteAsync_WithInvalidTitle_ThrowsDomainValidationException(string? invalidTitle)
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new UpdateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork,
            validator: _validator);

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
        DomainValidationException exception = await Assert.ThrowsAsync<DomainValidationException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Contains(expectedSubstring: "O título da categoria não pode ser vazio.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ExecuteAsync_WithInvalidDescription_ThrowsDomainValidationException(string? invalidDescription)
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new UpdateCategoryUseCase(
            categoryRepository: repository,
            unitOfWork: unitOfWork,
            validator: _validator);

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
        DomainValidationException exception = await Assert.ThrowsAsync<DomainValidationException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Contains(expectedSubstring: "A descrição da categoria não pode ser vazia.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
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
            unitOfWork: unitOfWork,
            validator: _validator);

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
