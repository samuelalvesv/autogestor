using Autogestor.Domain.Entities;
using Autogestor.Domain.Enums;
using Autogestor.Infrastructure.Persistence;
using Autogestor.Infrastructure.Persistence.Repositories;
using Autogestor.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Autogestor.IntegrationTests.Persistence;

[Collection(name: "PostgreSql")]
public class TransactionRepositoryTests(PostgreSqlFixture fixture)
{
    [Fact]
    public async Task AddAsync_PersistsTransactionToPostgreSqlDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        var tenantContext = new TenantContextFake(tenantId: tenantId);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var categoryRepo = new CategoryRepository(context: context);
        var transactionRepo = new TransactionRepository(context: context);

        var category = Category.Create(
            title: "Receitas Diversas",
            description: "Categoria para receitas variadas");
        await categoryRepo.AddAsync(category: category, cancellationToken: TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        var transaction = Transaction.Create(
            title: "Depósito Inicial",
            type: ETransactionType.Deposit,
            amount: 2500.50m,
            categoryId: category.Id);

        // Act
        await transactionRepo.AddAsync(transaction: transaction, cancellationToken: TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert - consulta em novo contexto sem cache com o mesmo tenant
        await using AppDbContext verifyContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var verifyRepo = new TransactionRepository(context: verifyContext);
        Transaction? persisted = await verifyRepo.GetByIdAsync(id: transaction.Id, cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(@object: persisted);
        Assert.Equal(expected: transaction.Id, actual: persisted.Id);
        Assert.Equal(expected: "Depósito Inicial", actual: persisted.Title);
        Assert.Equal(expected: ETransactionType.Deposit, actual: persisted.Type);
        Assert.Equal(expected: 2500.50m, actual: persisted.Amount);
        Assert.Equal(expected: category.Id, actual: persisted.CategoryId);
        Assert.Equal(expected: tenantId, actual: persisted.TenantId);
        Assert.Equal(expected: userId, actual: persisted.CreatedBy);
        Assert.NotEqual(expected: default, actual: persisted.CreatedAt);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPersistedTransactionsFromDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        var tenantContext = new TenantContextFake(tenantId: tenantId);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var categoryRepo = new CategoryRepository(context: context);
        var transactionRepo = new TransactionRepository(context: context);

        var category = Category.Create(title: "Operacional", description: "Custos operacionais");
        await categoryRepo.AddAsync(category: category, cancellationToken: TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        var tx1 = Transaction.Create(title: "Tx 1", type: ETransactionType.Deposit, amount: 100.00m, categoryId: category.Id);
        var tx2 = Transaction.Create(title: "Tx 2", type: ETransactionType.Withdraw, amount: 50.00m, categoryId: category.Id);

        await transactionRepo.AddAsync(transaction: tx1, cancellationToken: TestContext.Current.CancellationToken);
        await transactionRepo.AddAsync(transaction: tx2, cancellationToken: TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act - consulta em novo contexto com o mesmo tenant
        await using AppDbContext queryContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var queryRepo = new TransactionRepository(context: queryContext);
        IReadOnlyList<Transaction> all = await queryRepo.GetAllAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Contains(collection: all, filter: t => t.Id == tx1.Id);
        Assert.Contains(collection: all, filter: t => t.Id == tx2.Id);
    }

    [Fact]
    public async Task GetAllAsync_WhenQueriedByDifferentTenant_DoesNotReturnTransactionsFromOtherTenant()
    {
        // Arrange
        var tenant1 = Guid.NewGuid();
        var tenant2 = Guid.NewGuid();
        var tenantContext1 = new TenantContextFake(tenantId: tenant1);
        var tenantContext2 = new TenantContextFake(tenantId: tenant2);

        await using AppDbContext contextTenant1 = fixture.CreateContext(tenantContext: tenantContext1);
        var catRepoTenant1 = new CategoryRepository(context: contextTenant1);
        var txRepoTenant1 = new TransactionRepository(context: contextTenant1);

        var catTenant1 = Category.Create(title: "Cat Tenant 1", description: "Desc");
        await catRepoTenant1.AddAsync(category: catTenant1, cancellationToken: TestContext.Current.CancellationToken);
        await contextTenant1.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        var txTenant1 = Transaction.Create(title: "Tx Tenant 1", type: ETransactionType.Deposit, amount: 1000.00m, categoryId: catTenant1.Id);
        await txRepoTenant1.AddAsync(transaction: txTenant1, cancellationToken: TestContext.Current.CancellationToken);
        await contextTenant1.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act - consulta com o contexto do Tenant 2
        await using AppDbContext contextTenant2 = fixture.CreateContext(tenantContext: tenantContext2);
        var txRepoTenant2 = new TransactionRepository(context: contextTenant2);
        IReadOnlyList<Transaction> transactionsTenant2 = await txRepoTenant2.GetAllAsync(cancellationToken: TestContext.Current.CancellationToken);
        Transaction? transactionById = await txRepoTenant2.GetByIdAsync(id: txTenant1.Id, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - isolamento absoluto garantido por Global Query Filter
        Assert.DoesNotContain(collection: transactionsTenant2, filter: t => t.Id == txTenant1.Id);
        Assert.Null(@object: transactionById);
    }

    [Fact]
    public async Task GetByIdAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        await using AppDbContext context = fixture.CreateContext();
        var repository = new TransactionRepository(context: context);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            testCode: () => repository.GetByIdAsync(id: Guid.NewGuid(), cancellationToken: cts.Token));
    }

    [Fact]
    public async Task GetAllAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        await using AppDbContext context = fixture.CreateContext();
        var repository = new TransactionRepository(context: context);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            testCode: () => repository.GetAllAsync(cancellationToken: cts.Token));
    }

    [Fact]
    public async Task GetByIdAsync_WhenTransactionDoesNotExist_ReturnsNull()
    {
        await using AppDbContext context = fixture.CreateContext();
        var repository = new TransactionRepository(context: context);

        Transaction? result = await repository.GetByIdAsync(
            id: Guid.NewGuid(),
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Null(@object: result);
    }

    [Fact]
    public async Task DeleteCategory_WhenTransactionExists_ThrowsDbUpdateExceptionDueToRestrict()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        var tenantContext = new TenantContextFake(tenantId: tenantId);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var categoryRepo = new CategoryRepository(context: context);
        var transactionRepo = new TransactionRepository(context: context);

        var category = Category.Create(title: "Alimentação", description: "Gastos com alimentação");
        await categoryRepo.AddAsync(category: category, cancellationToken: TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        var transaction = Transaction.Create(
            title: "Supermercado",
            type: ETransactionType.Withdraw,
            amount: 350.00m,
            categoryId: category.Id);
        await transactionRepo.AddAsync(transaction: transaction, cancellationToken: TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act & Assert - tentar remover a categoria em novo contexto (onde a transação não está em memória) deve falhar no banco via foreign key
        await using AppDbContext deleteContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        Category categoryToDelete = await deleteContext.Categories.FirstAsync(
            predicate: c => c.Id == category.Id,
            cancellationToken: TestContext.Current.CancellationToken);
        deleteContext.Categories.Remove(entity: categoryToDelete);
        await Assert.ThrowsAsync<DbUpdateException>(
            testCode: () => deleteContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken));
    }
}
