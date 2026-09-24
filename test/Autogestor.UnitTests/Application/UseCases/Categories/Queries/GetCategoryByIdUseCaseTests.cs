using Autogestor.Application.UseCases.Categories.Queries.GetCategoryById;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.UnitTests.Common.Fakes;

namespace Autogestor.UnitTests.Application.UseCases.Categories.Queries;

public sealed class GetCategoryByIdUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenCategoryExists_ReturnsSuccessResponseWithData()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetCategoryByIdUseCase(categoryRepository: repository);

        var category = Category.Create(
            title: "Alimentação",
            description: "Despesas com restaurantes e compras");

        repository.Add(category: category);

        var request = new GetCategoryByIdRequest
        {
            Id = category.Id
        };

        // Act
        Response<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Equal(expected: "Categoria encontrada com sucesso.", actual: response.Message);
        Assert.Equal(expected: category.Id, actual: response.Data.Id);
        Assert.Equal(expected: category.Title, actual: response.Data.Title);
        Assert.Equal(expected: category.Description, actual: response.Data.Description);
        Assert.Equal(expected: category.Active, actual: response.Data.Active);
        Assert.Equal(expected: category.TenantId, actual: response.Data.TenantId);
        Assert.Equal(expected: category.CreatedBy, actual: response.Data.CreatedBy);
        Assert.Equal(expected: category.CreatedAt, actual: response.Data.CreatedAt);
        Assert.Equal(expected: category.UpdatedBy, actual: response.Data.UpdatedBy);
        Assert.Equal(expected: category.UpdatedAt, actual: response.Data.UpdatedAt);
        Assert.True(condition: repository.LastGetByIdAsNoTracking.GetValueOrDefault(), userMessage: "A consulta deve ser executada com asNoTracking habilitado.");
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoryNotFound_ReturnsNotFoundResponse()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetCategoryByIdUseCase(categoryRepository: repository);

        var request = new GetCategoryByIdRequest
        {
            Id = Guid.NewGuid()
        };

        // Act
        Response<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Null(@object: response.Data);
        Assert.Equal(expected: "Categoria não encontrada.", actual: response.Message);
        Assert.True(condition: repository.LastGetByIdAsNoTracking.GetValueOrDefault(), userMessage: "A consulta deve ser executada com asNoTracking habilitado mesmo quando a entidade não for encontrada.");
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoryHasBeenUpdated_ReturnsSuccessResponseWithUpdatedAuditFields()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetCategoryByIdUseCase(categoryRepository: repository);

        var category = Category.Create(
            title: "Educação",
            description: "Cursos e faculdade");
        repository.Add(category: category);

        var updatedBy = Guid.NewGuid();
        DateTime updatedAt = DateTime.UtcNow;
        EntityPersistenceHelper.SetAuditUpdateFields(
            entity: category,
            updatedBy: updatedBy,
            updatedAt: updatedAt);

        var request = new GetCategoryByIdRequest
        {
            Id = category.Id
        };

        // Act
        Response<CategoryResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Equal(expected: "Categoria encontrada com sucesso.", actual: response.Message);
        Assert.Equal(expected: category.Id, actual: response.Data.Id);
        Assert.Equal(expected: updatedBy, actual: response.Data.UpdatedBy);
        Assert.Equal(expected: updatedAt, actual: response.Data.UpdatedAt);
        Assert.True(condition: repository.LastGetByIdAsNoTracking.GetValueOrDefault(), userMessage: "A consulta deve ser executada com asNoTracking habilitado.");
    }

    [Fact]
    public async Task ExecuteAsync_PropagatesCancellationToken()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetCategoryByIdUseCase(categoryRepository: repository);

        var request = new GetCategoryByIdRequest
        {
            Id = Guid.NewGuid()
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await useCase.ExecuteAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: repository.PassedCancellationToken);
    }
}
