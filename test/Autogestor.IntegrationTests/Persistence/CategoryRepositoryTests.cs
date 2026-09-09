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
        var tenantId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        var tenantContext = new TenantContextFake(tenantId: tenantId);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var repository = new CategoryRepository(context: context);

        var category = Category.Create(
            title: "Alimentação",
            description: "Supermercados e restaurantes");

        // Act
        await repository.AddAsync(category: category);
        await context.SaveChangesAsync();

        // Assert - consulta em novo contexto sem cache com o mesmo tenant
        await using AppDbContext verifyContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var verifyRepo = new CategoryRepository(context: verifyContext);
        Category? persisted = await verifyRepo.GetByIdAsync(id: category.Id);

        Assert.NotNull(@object: persisted);
        Assert.Equal(expected: category.Id, actual: persisted.Id);
        Assert.Equal(expected: "Alimentação", actual: persisted.Title);
        Assert.Equal(expected: "Supermercados e restaurantes", actual: persisted.Description);
        Assert.Equal(expected: tenantId, actual: persisted.TenantId);
        Assert.Equal(expected: userId, actual: persisted.CreatedBy);
        Assert.NotEqual(expected: default, actual: persisted.CreatedAt);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPersistedCategoriesFromDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        var tenantContext = new TenantContextFake(tenantId: tenantId);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var repository = new CategoryRepository(context: context);

        var cat1 = Category.Create(title: "Cat 1", description: "Desc 1");
        var cat2 = Category.Create(title: "Cat 2", description: "Desc 2");

        await repository.AddAsync(category: cat1);
        await repository.AddAsync(category: cat2);
        await context.SaveChangesAsync();

        // Act - consulta em novo contexto com o mesmo tenant
        await using AppDbContext queryContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var queryRepo = new CategoryRepository(context: queryContext);
        IReadOnlyList<Category> all = await queryRepo.GetAllAsync();

        // Assert
        Assert.Contains(collection: all, filter: c => c.Id == cat1.Id);
        Assert.Contains(collection: all, filter: c => c.Id == cat2.Id);
    }

    [Fact]
    public async Task GetAllAsync_WhenQueriedByDifferentTenant_DoesNotReturnCategoriesFromOtherTenant()
    {
        // Arrange
        var tenant1 = Guid.NewGuid();
        var tenant2 = Guid.NewGuid();
        var tenantContext1 = new TenantContextFake(tenantId: tenant1);
        var tenantContext2 = new TenantContextFake(tenantId: tenant2);

        await using AppDbContext contextTenant1 = fixture.CreateContext(tenantContext: tenantContext1);
        var repoTenant1 = new CategoryRepository(context: contextTenant1);
        var catTenant1 = Category.Create(title: "Cat Tenant 1", description: "Desc");
        await repoTenant1.AddAsync(category: catTenant1);
        await contextTenant1.SaveChangesAsync();

        // Act - consulta com o contexto do Tenant 2
        await using AppDbContext contextTenant2 = fixture.CreateContext(tenantContext: tenantContext2);
        var repoTenant2 = new CategoryRepository(context: contextTenant2);
        IReadOnlyList<Category> categoriesTenant2 = await repoTenant2.GetAllAsync();
        Category? categoryById = await repoTenant2.GetByIdAsync(id: catTenant1.Id);

        // Assert - isolamento absoluto garantido por Global Query Filter
        Assert.DoesNotContain(collection: categoriesTenant2, filter: c => c.Id == catTenant1.Id);
        Assert.Null(@object: categoryById);
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
