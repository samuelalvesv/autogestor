using Autogestor.Application.UseCases.Categories.Queries.GetAllCategories;
using Autogestor.Application.Validators.Categories;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.UnitTests.Common.Fakes;

namespace Autogestor.UnitTests.Application.UseCases.Categories.Queries;

public sealed class GetAllCategoriesUseCaseTests
{
    private readonly GetAllCategoriesRequestValidator _validator = new();

    [Fact]
    public async Task ExecuteAsync_WhenCategoriesExist_ReturnsPagedResponseWithMappedData()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(
            categoryRepository: repository,
            validator: _validator);

        var category1 = Category.Create(
            title: "Alimentação",
            description: "Despesas com alimentação e supermercado");
        var category2 = Category.Create(
            title: "Transporte",
            description: "Combustível e transporte público");

        repository.Add(category: category1);
        repository.Add(category: category2);

        var request = new GetAllCategoriesRequest
        {
            PageSize = 10
        };

        // Act
        PagedResponse<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Equal(expected: 2, actual: response.Data.Count);
        Assert.False(condition: response.HasNextPage, userMessage: "Com apenas 2 itens e pageSize 10, não deve haver próxima página.");
        Assert.Null(@object: response.NextCursor);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidPageSize_ThrowsDomainValidationException()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(
            categoryRepository: repository,
            validator: _validator);

        var request = new GetAllCategoriesRequest
        {
            PageSize = 0
        };

        // Act & Assert
        DomainValidationException exception = await Assert.ThrowsAsync<DomainValidationException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Contains(expectedSubstring: "O tamanho da página deve estar entre", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoriesExist_MapsAllFieldsCorrectly()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(
            categoryRepository: repository,
            validator: _validator);

        var category = Category.Create(
            title: "Educação",
            description: "Cursos e mensalidades");
        repository.Add(category: category);

        var request = new GetAllCategoriesRequest
        {
            PageSize = 10
        };

        // Act
        PagedResponse<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(collection: response.Data);
        CategoryResponse item = response.Data[index: 0];
        Assert.Equal(expected: category.Id, actual: item.Id);
        Assert.Equal(expected: category.Title, actual: item.Title);
        Assert.Equal(expected: category.Description, actual: item.Description);
        Assert.True(condition: item.Active, userMessage: "A categoria retornada deve estar ativa.");
        Assert.Equal(expected: category.TenantId, actual: item.TenantId);
        Assert.Equal(expected: category.CreatedBy, actual: item.CreatedBy);
        Assert.Equal(expected: category.CreatedAt, actual: item.CreatedAt);
        Assert.Equal(expected: category.UpdatedBy, actual: item.UpdatedBy);
        Assert.Equal(expected: category.UpdatedAt, actual: item.UpdatedAt);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoCategoriesExist_ReturnsEmptyPagedResponse()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(
            categoryRepository: repository,
            validator: _validator);

        var request = new GetAllCategoriesRequest
        {
            PageSize = 10
        };

        // Act
        PagedResponse<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Empty(collection: response.Data);
        Assert.False(condition: response.HasNextPage, userMessage: "Lista vazia não deve indicar próxima página.");
        Assert.Null(@object: response.NextCursor);
    }

    [Fact]
    public async Task ExecuteAsync_WhenMoreItemsThanPageSize_ReturnsHasNextPageTrue()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(
            categoryRepository: repository,
            validator: _validator);

        for (int i = 0; i < 15; i++)
        {
            repository.Add(category: Category.Create(
                title: $"Cat {i}",
                description: $"Desc {i}"));
        }

        var request = new GetAllCategoriesRequest
        {
            PageSize = 10
        };

        // Act
        PagedResponse<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expected: 10, actual: response.Data.Count);
        Assert.True(condition: response.HasNextPage, userMessage: "Com 15 itens e pageSize 10, deve haver próxima página.");
        Assert.NotNull(@object: response.NextCursor);
    }

    [Fact]
    public async Task ExecuteAsync_WhenExactlyPageSizeItems_ReturnsHasNextPageFalse()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(
            categoryRepository: repository,
            validator: _validator);

        for (int i = 0; i < 10; i++)
        {
            repository.Add(category: Category.Create(
                title: $"Cat {i}",
                description: $"Desc {i}"));
        }

        var request = new GetAllCategoriesRequest
        {
            PageSize = 10
        };

        // Act
        PagedResponse<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expected: 10, actual: response.Data.Count);
        Assert.False(condition: response.HasNextPage, userMessage: "Com exatamente 10 itens e pageSize 10, não deve haver próxima página.");
        Assert.Null(@object: response.NextCursor);
    }

    [Fact]
    public async Task ExecuteAsync_WithCursor_PropagatesCursorToRepository()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(
            categoryRepository: repository,
            validator: _validator);
        var cursor = Guid.NewGuid();

        var request = new GetAllCategoriesRequest
        {
            Cursor = cursor,
            PageSize = 10
        };

        // Act
        await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expected: cursor, actual: repository.LastPagedCursor);
        Assert.Equal(expected: 10, actual: repository.LastPagedPageSize);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullCursor_PropagatesNullCursorToRepository()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(
            categoryRepository: repository,
            validator: _validator);

        var request = new GetAllCategoriesRequest
        {
            PageSize = 10
        };

        // Act
        await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(@object: repository.LastPagedCursor);
        Assert.Equal(expected: 10, actual: repository.LastPagedPageSize);
    }

    [Fact]
    public async Task ExecuteAsync_PropagatesCancellationToken()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(
            categoryRepository: repository,
            validator: _validator);

        var request = new GetAllCategoriesRequest
        {
            PageSize = 10
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await useCase.ExecuteAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: repository.PassedCancellationToken);
    }

    [Fact]
    public async Task ExecuteAsync_NextCursor_IsLastItemIdOfReturnedPage()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(
            categoryRepository: repository,
            validator: _validator);

        for (int i = 0; i < 15; i++)
        {
            repository.Add(category: Category.Create(
                title: $"Cat {i}",
                description: $"Desc {i}"));
        }

        var request = new GetAllCategoriesRequest
        {
            PageSize = 10
        };

        // Act
        PagedResponse<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.True(condition: response.HasNextPage, userMessage: "Com 15 itens e pageSize 10, deve haver próxima página.");
        Assert.NotNull(@object: response.NextCursor);
        Assert.Equal(expected: response.Data[^1].Id, actual: response.NextCursor);
    }
}
