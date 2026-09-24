using Autogestor.Domain.Entities;
using Autogestor.Infrastructure.Persistence;
using Autogestor.Infrastructure.Persistence.Repositories;
using Autogestor.IntegrationTests.Fixtures;

namespace Autogestor.IntegrationTests.Persistence;

[Collection(name: "PostgreSql")]
public sealed class CategoryRepositoryTests(PostgreSqlFixture fixture)
{
    [Fact]
    public async Task Add_PersistsCategoryToPostgreSqlDatabase()
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
        repository.Add(category: category);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert - consulta em novo contexto sem cache com o mesmo tenant
        await using AppDbContext verifyContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var verifyRepo = new CategoryRepository(context: verifyContext);
        Category? persisted = await verifyRepo.GetByIdAsync(id: category.Id, cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(@object: persisted);
        Assert.Equal(expected: category.Id, actual: persisted.Id);
        Assert.Equal(expected: "Alimentação", actual: persisted.Title);
        Assert.Equal(expected: "Supermercados e restaurantes", actual: persisted.Description);
        Assert.Equal(expected: tenantId, actual: persisted.TenantId);
        Assert.Equal(expected: userId, actual: persisted.CreatedBy);
        Assert.NotEqual(expected: default, actual: persisted.CreatedAt);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsPersistedCategoriesFromDatabase()
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

        repository.Add(category: cat1);
        repository.Add(category: cat2);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act - consulta em novo contexto com o mesmo tenant
        await using AppDbContext queryContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var queryRepo = new CategoryRepository(context: queryContext);
        (IReadOnlyList<Category> all, int count) = await queryRepo.GetPagedAsync(skip: 0, pageSize: 25, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.True(condition: count >= 2, userMessage: "A contagem total de categorias deve ser pelo menos 2.");
        Assert.Contains(collection: all, filter: c => c.Id == cat1.Id);
        Assert.Contains(collection: all, filter: c => c.Id == cat2.Id);
    }

    [Fact]
    public async Task GetPagedAsync_WithMultiplePages_RespectsPageSizeAndSkip()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        var tenantContext = new TenantContextFake(tenantId: tenantId);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var repository = new CategoryRepository(context: context);

        var cat1 = Category.Create(title: "Cat Page 1", description: "Desc 1");
        var cat2 = Category.Create(title: "Cat Page 2", description: "Desc 2");
        var cat3 = Category.Create(title: "Cat Page 3", description: "Desc 3");

        repository.Add(category: cat1);
        repository.Add(category: cat2);
        repository.Add(category: cat3);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await using AppDbContext queryContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var queryRepo = new CategoryRepository(context: queryContext);
        (IReadOnlyList<Category> page1, int count1) = await queryRepo.GetPagedAsync(skip: 0, pageSize: 2, cancellationToken: TestContext.Current.CancellationToken);
        (IReadOnlyList<Category> page2, int count2) = await queryRepo.GetPagedAsync(skip: 2, pageSize: 2, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.True(condition: count1 >= 3, userMessage: "A contagem total deve ser pelo menos 3.");
        Assert.True(condition: count2 >= 3, userMessage: "A contagem total da segunda página deve ser pelo menos 3.");
        Assert.Equal(expected: 2, actual: page1.Count);
        Assert.True(condition: page2.Count >= 1, userMessage: "A segunda página deve conter ao menos o registro restante.");
    }

    [Fact]
    public async Task GetPagedAsync_WhenSkipExceedsOrEqualsTotalCount_ReturnsEmptyListAndTotalCount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        var tenantContext = new TenantContextFake(tenantId: tenantId);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var repository = new CategoryRepository(context: context);

        var cat1 = Category.Create(title: "Cat Exceed 1", description: "Desc 1");
        var cat2 = Category.Create(title: "Cat Exceed 2", description: "Desc 2");
        repository.Add(category: cat1);
        repository.Add(category: cat2);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await using AppDbContext queryContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var queryRepo = new CategoryRepository(context: queryContext);
        (IReadOnlyList<Category> emptyPage, int countExceeded) = await queryRepo.GetPagedAsync(skip: 100, pageSize: 10, cancellationToken: TestContext.Current.CancellationToken);
        (IReadOnlyList<Category> maxIntPage, int countMaxInt) = await queryRepo.GetPagedAsync(skip: int.MaxValue, pageSize: 10, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Empty(collection: emptyPage);
        Assert.True(condition: countExceeded >= 2, userMessage: "A contagem total deve permanecer preservada no curto-circuito.");
        Assert.Empty(collection: maxIntPage);
        Assert.True(condition: countMaxInt >= 2, userMessage: "A contagem total deve permanecer preservada para skip com int.MaxValue.");
    }

    [Fact]
    public async Task GetPagedAsync_WhenQueriedByDifferentTenant_DoesNotReturnCategoriesFromOtherTenant()
    {
        // Arrange
        var tenant1 = Guid.NewGuid();
        var tenant2 = Guid.NewGuid();
        var tenantContext1 = new TenantContextFake(tenantId: tenant1);
        var tenantContext2 = new TenantContextFake(tenantId: tenant2);

        await using AppDbContext contextTenant1 = fixture.CreateContext(tenantContext: tenantContext1);
        var repoTenant1 = new CategoryRepository(context: contextTenant1);
        var catTenant1 = Category.Create(title: "Cat Tenant 1", description: "Desc");
        repoTenant1.Add(category: catTenant1);
        await contextTenant1.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act - consulta com o contexto do Tenant 2
        await using AppDbContext contextTenant2 = fixture.CreateContext(tenantContext: tenantContext2);
        var repoTenant2 = new CategoryRepository(context: contextTenant2);
        (IReadOnlyList<Category> categoriesTenant2, int countTenant2) = await repoTenant2.GetPagedAsync(skip: 0, pageSize: 25, cancellationToken: TestContext.Current.CancellationToken);
        Category? categoryById = await repoTenant2.GetByIdAsync(id: catTenant1.Id, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - isolamento absoluto garantido por Global Query Filter
        Assert.Empty(collection: categoriesTenant2);
        Assert.Equal(expected: 0, actual: countTenant2);
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
    public async Task GetPagedAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        await using AppDbContext context = fixture.CreateContext();
        var repository = new CategoryRepository(context: context);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            testCode: () => repository.GetPagedAsync(skip: 0, pageSize: 25, cancellationToken: cts.Token));
    }

    [Fact]
    public async Task ExistsAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        await using AppDbContext context = fixture.CreateContext();
        var repository = new CategoryRepository(context: context);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            testCode: () => repository.ExistsAsync(id: Guid.NewGuid(), cancellationToken: cts.Token));
    }

    [Fact]
    public async Task Update_ViaChangeTracking_PersistsUpdatedCategoryToPostgreSqlDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        var tenantContext = new TenantContextFake(tenantId: tenantId);

        var initialCategory = Category.Create(
            title: "Alimentação",
            description: "Supermercados e restaurantes");

        await using (AppDbContext setupContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext))
        {
            var setupRepo = new CategoryRepository(context: setupContext);
            setupRepo.Add(category: initialCategory);
            await setupContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);
        }

        var updateUserId = Guid.NewGuid();
        var updateUserContext = new UserContextFake(userId: updateUserId);

        // Act - carregar a entidade rastreada, modificar via método de domínio e salvar via SaveChangesAsync
        await using (AppDbContext updateContext = fixture.CreateContext(userContext: updateUserContext, tenantContext: tenantContext))
        {
            var updateRepo = new CategoryRepository(context: updateContext);
            Category? categoryToUpdate = await updateRepo.GetByIdAsync(id: initialCategory.Id, cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(@object: categoryToUpdate);

            categoryToUpdate.Update(
                title: "Alimentação & Mercado",
                description: "Supermercados, feiras e restaurantes");

            await updateContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);
        }

        // Assert - consultar em novo contexto sem cache e validar campos de negócio e auditoria
        await using AppDbContext verifyContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var verifyRepo = new CategoryRepository(context: verifyContext);
        Category? updated = await verifyRepo.GetByIdAsync(id: initialCategory.Id, cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(@object: updated);
        Assert.Equal(expected: initialCategory.Id, actual: updated.Id);
        Assert.Equal(expected: "Alimentação & Mercado", actual: updated.Title);
        Assert.Equal(expected: "Supermercados, feiras e restaurantes", actual: updated.Description);
        Assert.Equal(expected: tenantId, actual: updated.TenantId);
        Assert.Equal(expected: userId, actual: updated.CreatedBy);
        Assert.Equal(expected: updateUserId, actual: updated.UpdatedBy);
        Assert.NotNull(@object: updated.UpdatedAt);
    }

    [Fact]
    public async Task ExistsAsync_WhenCategoryExistsForTenant_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        var tenantContext = new TenantContextFake(tenantId: tenantId);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var repository = new CategoryRepository(context: context);

        var category = Category.Create(title: "Saúde", description: "Farmácia e consultas");
        repository.Add(category: category);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await using AppDbContext verifyContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var verifyRepo = new CategoryRepository(context: verifyContext);
        bool exists = await verifyRepo.ExistsAsync(id: category.Id, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.True(condition: exists, userMessage: "A categoria existente deve retornar true.");
    }

    [Fact]
    public async Task ExistsAsync_WhenCategoryDoesNotExistOrBelongsToAnotherTenant_ReturnsFalse()
    {
        // Arrange
        var tenant1 = Guid.NewGuid();
        var tenant2 = Guid.NewGuid();
        await using AppDbContext contextTenant1 = fixture.CreateContext(userContext: new UserContextFake(userId: Guid.NewGuid()), tenantContext: new TenantContextFake(tenantId: tenant1));
        var repoTenant1 = new CategoryRepository(context: contextTenant1);

        var category = Category.Create(title: "Lazer", description: "Viagens e passeios");
        repoTenant1.Add(category: category);
        await contextTenant1.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act - consultar sob Tenant 2
        await using AppDbContext contextTenant2 = fixture.CreateContext(userContext: new UserContextFake(userId: Guid.NewGuid()), tenantContext: new TenantContextFake(tenantId: tenant2));
        var repoTenant2 = new CategoryRepository(context: contextTenant2);
        bool existsForOtherTenant = await repoTenant2.ExistsAsync(id: category.Id, cancellationToken: TestContext.Current.CancellationToken);
        bool existsForRandomId = await repoTenant2.ExistsAsync(id: Guid.NewGuid(), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: existsForOtherTenant, userMessage: "A categoria de outro tenant não deve ser encontrada.");
        Assert.False(condition: existsForRandomId, userMessage: "Um id inexistente deve retornar false.");
    }

    [Fact]
    public async Task Remove_RemovesCategoryFromPostgreSqlDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        var tenantContext = new TenantContextFake(tenantId: tenantId);

        var category = Category.Create(title: "Assinaturas", description: "Serviços mensais");

        await using (AppDbContext setupContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext))
        {
            var setupRepo = new CategoryRepository(context: setupContext);
            setupRepo.Add(category: category);
            await setupContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);
        }

        // Act - carregar no repositório, chamar Remove e salvar alterações
        await using (AppDbContext removeContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext))
        {
            var removeRepo = new CategoryRepository(context: removeContext);
            Category? categoryToRemove = await removeRepo.GetByIdAsync(id: category.Id, cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(@object: categoryToRemove);

            removeRepo.Remove(category: categoryToRemove);
            await removeContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);
        }

        // Assert - consultar em novo contexto e verificar que não existe mais
        await using AppDbContext verifyContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var verifyRepo = new CategoryRepository(context: verifyContext);
        Category? deletedCategory = await verifyRepo.GetByIdAsync(id: category.Id, cancellationToken: TestContext.Current.CancellationToken);
        bool exists = await verifyRepo.ExistsAsync(id: category.Id, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Null(@object: deletedCategory);
        Assert.False(condition: exists, userMessage: "A categoria removida não deve mais existir.");
    }

    [Fact]
    public async Task GetByIdAsync_WithAsNoTracking_ReturnsUntrackedEntity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        var tenantContext = new TenantContextFake(tenantId: tenantId);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var repository = new CategoryRepository(context: context);

        var category = Category.Create(title: "Investimentos", description: "Ações e fundos");
        repository.Add(category: category);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await using AppDbContext queryContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var queryRepo = new CategoryRepository(context: queryContext);
        Category? tracked = await queryRepo.GetByIdAsync(id: category.Id, asNoTracking: false, cancellationToken: TestContext.Current.CancellationToken);

        await using AppDbContext noTrackingContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var noTrackingRepo = new CategoryRepository(context: noTrackingContext);
        Category? untracked = await noTrackingRepo.GetByIdAsync(id: category.Id, asNoTracking: true, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: tracked);
        Assert.NotNull(@object: untracked);
        Assert.Equal(expected: Microsoft.EntityFrameworkCore.EntityState.Unchanged, actual: queryContext.Entry(entity: tracked).State);
        Assert.Equal(expected: Microsoft.EntityFrameworkCore.EntityState.Detached, actual: noTrackingContext.Entry(entity: untracked).State);
    }
}
