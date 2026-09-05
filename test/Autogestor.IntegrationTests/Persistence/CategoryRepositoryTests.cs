using Autogestor.Domain.Entities;
using Autogestor.Infrastructure.Persistence;
using Autogestor.Infrastructure.Persistence.Repositories;
using Autogestor.IntegrationTests.Fixtures;

namespace Autogestor.IntegrationTests.Persistence;

[Collection(name: "PostgreSql")]
public class CategoryRepositoryTests(PostgreSqlFixture fixture)
{
    [Fact]
    public async Task AddAsync_PersistsCategoryToPostgreSqlDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext);
        var repository = new CategoryRepository(context: context);

        var category = Category.Create(
            title: "Alimentação",
            description: "Supermercados e restaurantes",
            userId: userId);

        // Act
        await repository.AddAsync(category: category);
        await context.SaveChangesAsync();

        // Assert - consulta em novo contexto sem cache
        await using AppDbContext verifyContext = fixture.CreateContext();
        var verifyRepo = new CategoryRepository(context: verifyContext);
        Category? persisted = await verifyRepo.GetByIdAsync(id: category.Id);

        Assert.NotNull(@object: persisted);
        Assert.Equal(expected: category.Id, actual: persisted.Id);
        Assert.Equal(expected: "Alimentação", actual: persisted.Title);
        Assert.Equal(expected: "Supermercados e restaurantes", actual: persisted.Description);
        Assert.Equal(expected: userId, actual: persisted.UserId);
        Assert.Equal(expected: userId, actual: persisted.CreatedBy);
        Assert.NotEqual(expected: default, actual: persisted.CreatedAt);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPersistedCategoriesFromDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext);
        var repository = new CategoryRepository(context: context);

        var cat1 = Category.Create(title: "Cat 1", description: "Desc 1", userId: userId);
        var cat2 = Category.Create(title: "Cat 2", description: "Desc 2", userId: userId);

        await repository.AddAsync(category: cat1);
        await repository.AddAsync(category: cat2);
        await context.SaveChangesAsync();

        // Act - consulta em novo contexto
        await using AppDbContext queryContext = fixture.CreateContext();
        var queryRepo = new CategoryRepository(context: queryContext);
        IReadOnlyList<Category> all = await queryRepo.GetAllAsync();

        // Assert
        Assert.Contains(collection: all, filter: c => c.Id == cat1.Id);
        Assert.Contains(collection: all, filter: c => c.Id == cat2.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        await using AppDbContext context = fixture.CreateContext();
        var repository = new CategoryRepository(context: context);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            testCode: () => repository.GetByIdAsync(id: Guid.NewGuid(), cancellationToken: cts.Token));
    }

    [Fact]
    public async Task GetAllAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        await using AppDbContext context = fixture.CreateContext();
        var repository = new CategoryRepository(context: context);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            testCode: () => repository.GetAllAsync(cancellationToken: cts.Token));
    }
}
