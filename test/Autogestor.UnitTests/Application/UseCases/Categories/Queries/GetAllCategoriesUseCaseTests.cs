using Autogestor.Application.UseCases.Categories.Queries.GetAllCategories;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.UnitTests.Common.Fakes;

namespace Autogestor.UnitTests.Application.UseCases.Categories.Queries;

public sealed class GetAllCategoriesUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenCategoriesExist_ReturnsPagedResponseWithMappedData()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(categoryRepository: repository);

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
            PageNumber = 1,
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
        Assert.Equal(expected: 2, actual: response.TotalCount);
        Assert.Equal(expected: 1, actual: response.PageNumber);
        Assert.Equal(expected: 10, actual: response.PageSize);

        CategoryResponse first = response.Data[index: 0];
        Assert.Equal(expected: category1.Id, actual: first.Id);
        Assert.Equal(expected: category1.Title, actual: first.Title);
        Assert.Equal(expected: category1.Description, actual: first.Description);
        Assert.True(condition: first.Active, userMessage: "A primeira categoria retornada deve estar ativa.");
        Assert.Equal(expected: category1.TenantId, actual: first.TenantId);
        Assert.Equal(expected: category1.CreatedBy, actual: first.CreatedBy);
        Assert.Equal(expected: category1.CreatedAt, actual: first.CreatedAt);
        Assert.Equal(expected: category1.UpdatedBy, actual: first.UpdatedBy);
        Assert.Equal(expected: category1.UpdatedAt, actual: first.UpdatedAt);

        CategoryResponse second = response.Data[index: 1];
        Assert.Equal(expected: category2.Id, actual: second.Id);
        Assert.Equal(expected: category2.Title, actual: second.Title);
        Assert.Equal(expected: category2.Description, actual: second.Description);
        Assert.True(condition: second.Active, userMessage: "A segunda categoria retornada deve estar ativa.");
        Assert.Equal(expected: category2.TenantId, actual: second.TenantId);
        Assert.Equal(expected: category2.CreatedBy, actual: second.CreatedBy);
        Assert.Equal(expected: category2.CreatedAt, actual: second.CreatedAt);
        Assert.Equal(expected: category2.UpdatedBy, actual: second.UpdatedBy);
        Assert.Equal(expected: category2.UpdatedAt, actual: second.UpdatedAt);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoryHasBeenUpdated_ReturnsPagedResponseWithUpdatedAuditFields()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(categoryRepository: repository);

        var category = Category.Create(
            title: "Educação",
            description: "Cursos e mensalidades");
        repository.Add(category: category);

        var updatedBy = Guid.NewGuid();
        DateTime updatedAt = DateTime.UtcNow;
        EntityPersistenceHelper.SetAuditUpdateFields(
            entity: category,
            updatedBy: updatedBy,
            updatedAt: updatedAt);

        var request = new GetAllCategoriesRequest
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        PagedResponse<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Single(collection: response.Data);

        CategoryResponse item = response.Data[index: 0];
        Assert.Equal(expected: category.Id, actual: item.Id);
        Assert.Equal(expected: updatedBy, actual: item.UpdatedBy);
        Assert.Equal(expected: updatedAt, actual: item.UpdatedAt);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoCategoriesExist_ReturnsEmptyPagedResponseWithEmptyData()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(categoryRepository: repository);

        var request = new GetAllCategoriesRequest
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        PagedResponse<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Empty(collection: response.Data);
        Assert.Equal(expected: 0, actual: response.TotalCount);
        Assert.Equal(expected: 1, actual: response.PageNumber);
        Assert.Equal(expected: 10, actual: response.PageSize);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPageExceedsCount_ReturnsEmptyPagedResponseWithEmptyData()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(categoryRepository: repository);

        var category = Category.Create(
            title: "Lazer",
            description: "Cinema e passeios");
        repository.Add(category: category);

        var request = new GetAllCategoriesRequest
        {
            PageNumber = 2,
            PageSize = 10
        };

        // Act
        PagedResponse<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Empty(collection: response.Data);
        Assert.Equal(expected: 1, actual: response.TotalCount);
        Assert.Equal(expected: 2, actual: response.PageNumber);
        Assert.Equal(expected: 10, actual: response.PageSize);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSubsequentPageHasData_ReturnsPagedResponseWithPagedSubset()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(categoryRepository: repository);

        var category1 = Category.Create(title: "Cat 1", description: "Desc 1");
        var category2 = Category.Create(title: "Cat 2", description: "Desc 2");
        var category3 = Category.Create(title: "Cat 3", description: "Desc 3");

        repository.Add(category: category1);
        repository.Add(category: category2);
        repository.Add(category: category3);

        var request = new GetAllCategoriesRequest
        {
            PageNumber = 2,
            PageSize = 2
        };

        // Act
        PagedResponse<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Single(collection: response.Data);
        Assert.Equal(expected: 3, actual: response.TotalCount);
        Assert.Equal(expected: 2, actual: response.PageNumber);
        Assert.Equal(expected: 2, actual: response.PageSize);
        Assert.Equal(expected: category3.Id, actual: response.Data[index: 0].Id);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPageOffsetExactlyEqualsCount_ReturnsEmptyPagedResponseWithEmptyData()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(categoryRepository: repository);

        var category1 = Category.Create(title: "Cat 1", description: "Desc 1");
        var category2 = Category.Create(title: "Cat 2", description: "Desc 2");

        repository.Add(category: category1);
        repository.Add(category: category2);

        var request = new GetAllCategoriesRequest
        {
            PageNumber = 2,
            PageSize = 2
        };

        // Act
        PagedResponse<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Empty(collection: response.Data);
        Assert.Equal(expected: 2, actual: response.TotalCount);
        Assert.Equal(expected: 2, actual: response.PageNumber);
        Assert.Equal(expected: 2, actual: response.PageSize);
    }

    [Fact]
    public async Task ExecuteAsync_PropagatesPaginationParametersToRepository()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(categoryRepository: repository);

        var request = new GetAllCategoriesRequest
        {
            PageNumber = 3,
            PageSize = 15
        };

        // Act
        PagedResponse<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expected: 30, actual: repository.LastPagedSkip);
        Assert.Equal(expected: 15, actual: repository.LastPagedPageSize);
        Assert.Equal(expected: 3, actual: response.PageNumber);
        Assert.Equal(expected: 15, actual: response.PageSize);
    }

    [Fact]
    public async Task ExecuteAsync_PropagatesCancellationToken()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(categoryRepository: repository);

        var request = new GetAllCategoriesRequest
        {
            PageNumber = 1,
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
    public async Task ExecuteAsync_WhenPageNumberIsMaxInt_ReturnsEmptyPagedResponseWithEmptyData()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetAllCategoriesUseCase(categoryRepository: repository);

        var category = Category.Create(title: "Alimentação", description: "Desc");
        repository.Add(category: category);

        var request = new GetAllCategoriesRequest
        {
            PageNumber = int.MaxValue,
            PageSize = 25
        };

        // Act
        PagedResponse<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Empty(collection: response.Data);
        Assert.Equal(expected: 1, actual: response.TotalCount);
        Assert.Equal(expected: int.MaxValue, actual: response.PageNumber);
        Assert.Equal(expected: 25, actual: response.PageSize);
        Assert.Equal(expected: int.MaxValue, actual: repository.LastPagedSkip);
    }
}
