using Autogestor.Application.UseCases.Categories.Queries.GetCategoryById;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.UnitTests.Common.Fakes;

namespace Autogestor.UnitTests.Application.UseCases.Categories.Queries;

public sealed class GetCategoryByIdUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenCategoryExists_ReturnsCategoryResponse()
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
        CategoryResponse response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Equal(expected: category.Id, actual: response.Id);
        Assert.Equal(expected: category.Title, actual: response.Title);
        Assert.Equal(expected: category.Description, actual: response.Description);
        Assert.Equal(expected: category.Active, actual: response.Active);
        Assert.Equal(expected: category.TenantId, actual: response.TenantId);
        Assert.Equal(expected: category.CreatedBy, actual: response.CreatedBy);
        Assert.Equal(expected: category.CreatedAt, actual: response.CreatedAt);
        Assert.Equal(expected: category.UpdatedBy, actual: response.UpdatedBy);
        Assert.Equal(expected: category.UpdatedAt, actual: response.UpdatedAt);
        Assert.True(condition: repository.LastGetByIdAsNoTracking.GetValueOrDefault(), userMessage: "A consulta deve ser executada com asNoTracking habilitado.");
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoryNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var repository = new CategoryRepositoryFake();
        var useCase = new GetCategoryByIdUseCase(categoryRepository: repository);

        var request = new GetCategoryByIdRequest
        {
            Id = Guid.NewGuid()
        };

        // Act & Assert
        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(expected: "Categoria não encontrada.", actual: exception.Message);
        Assert.True(condition: repository.LastGetByIdAsNoTracking.GetValueOrDefault(), userMessage: "A consulta deve ser executada com asNoTracking habilitado mesmo quando a entidade não for encontrada.");
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoryHasBeenUpdated_ReturnsCategoryResponseWithUpdatedAuditFields()
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
        CategoryResponse response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Equal(expected: category.Id, actual: response.Id);
        Assert.Equal(expected: updatedBy, actual: response.UpdatedBy);
        Assert.Equal(expected: updatedAt, actual: response.UpdatedAt);
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

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            testCode: () => useCase.ExecuteAsync(request: request, cancellationToken: token));

        Assert.Equal(expected: token, actual: repository.PassedCancellationToken);
    }
}
