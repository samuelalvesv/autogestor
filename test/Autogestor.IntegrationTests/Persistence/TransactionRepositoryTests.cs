using Autogestor.Domain.Entities;
using Autogestor.Domain.Enums;
using Autogestor.Infrastructure.Persistence;
using Autogestor.Infrastructure.Persistence.Repositories;
using Autogestor.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Autogestor.IntegrationTests.Persistence;

[Collection(name: "PostgreSql")]
public sealed class TransactionRepositoryTests(PostgreSqlFixture fixture)
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
    public async Task GetPagedAsync_ReturnsPersistedTransactionsFromDatabase()
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
        IReadOnlyList<Transaction> all = await queryRepo.GetPagedAsync(pageNumber: 1, pageSize: 25, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Contains(collection: all, filter: t => t.Id == tx1.Id);
        Assert.Contains(collection: all, filter: t => t.Id == tx2.Id);
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
        var categoryRepo = new CategoryRepository(context: context);
        var transactionRepo = new TransactionRepository(context: context);

        var category = Category.Create(title: "Operacional Paginação", description: "Custos operacionais");
        await categoryRepo.AddAsync(category: category, cancellationToken: TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        var tx1 = Transaction.Create(title: "Tx Page 1", type: ETransactionType.Deposit, amount: 10.00m, categoryId: category.Id);
        var tx2 = Transaction.Create(title: "Tx Page 2", type: ETransactionType.Withdraw, amount: 20.00m, categoryId: category.Id);
        var tx3 = Transaction.Create(title: "Tx Page 3", type: ETransactionType.Deposit, amount: 30.00m, categoryId: category.Id);

        await transactionRepo.AddAsync(transaction: tx1, cancellationToken: TestContext.Current.CancellationToken);
        await transactionRepo.AddAsync(transaction: tx2, cancellationToken: TestContext.Current.CancellationToken);
        await transactionRepo.AddAsync(transaction: tx3, cancellationToken: TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await using AppDbContext queryContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var queryRepo = new TransactionRepository(context: queryContext);
        IReadOnlyList<Transaction> page1 = await queryRepo.GetPagedAsync(pageNumber: 1, pageSize: 2, cancellationToken: TestContext.Current.CancellationToken);
        IReadOnlyList<Transaction> page2 = await queryRepo.GetPagedAsync(pageNumber: 2, pageSize: 2, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expected: 2, actual: page1.Count);
        Assert.True(condition: page2.Count >= 1, userMessage: "A segunda página deve conter ao menos o registro restante.");
    }

    [Fact]
    public async Task GetPagedAsync_WhenQueriedByDifferentTenant_DoesNotReturnTransactionsFromOtherTenant()
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
        IReadOnlyList<Transaction> transactionsTenant2 = await txRepoTenant2.GetPagedAsync(pageNumber: 1, pageSize: 25, cancellationToken: TestContext.Current.CancellationToken);
        Transaction? transactionById = await txRepoTenant2.GetByIdAsync(id: txTenant1.Id, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - isolamento absoluto garantido por Global Query Filter
        Assert.Empty(collection: transactionsTenant2);
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
    public async Task GetPagedAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        await using AppDbContext context = fixture.CreateContext();
        var repository = new TransactionRepository(context: context);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            testCode: () => repository.GetPagedAsync(pageNumber: 1, pageSize: 25, cancellationToken: cts.Token));
    }

    [Fact]
    public async Task ExistsAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        await using AppDbContext context = fixture.CreateContext();
        var repository = new TransactionRepository(context: context);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            testCode: () => repository.ExistsAsync(id: Guid.NewGuid(), cancellationToken: cts.Token));
    }

    [Fact]
    public async Task ExistsByCategoryIdAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        await using AppDbContext context = fixture.CreateContext();
        var repository = new TransactionRepository(context: context);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            testCode: () => repository.ExistsByCategoryIdAsync(categoryId: Guid.NewGuid(), cancellationToken: cts.Token));
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

    [Fact]
    public async Task Update_ViaChangeTracking_PersistsUpdatedTransactionToPostgreSqlDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        var tenantContext = new TenantContextFake(tenantId: tenantId);

        var category1 = Category.Create(title: "Serviços", description: "Serviços gerais");
        var category2 = Category.Create(title: "Consultoria", description: "Consultoria estratégica");

        Guid transactionId;

        await using (AppDbContext setupContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext))
        {
            var catRepo = new CategoryRepository(context: setupContext);
            await catRepo.AddAsync(category: category1, cancellationToken: TestContext.Current.CancellationToken);
            await catRepo.AddAsync(category: category2, cancellationToken: TestContext.Current.CancellationToken);
            await setupContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

            var txRepo = new TransactionRepository(context: setupContext);
            var initialTransaction = Transaction.Create(
                title: "Prestação Inicial",
                type: ETransactionType.Deposit,
                amount: 1000.00m,
                categoryId: category1.Id);
            await txRepo.AddAsync(transaction: initialTransaction, cancellationToken: TestContext.Current.CancellationToken);
            await setupContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);
            transactionId = initialTransaction.Id;
        }

        var updateUserId = Guid.NewGuid();
        var updateUserContext = new UserContextFake(userId: updateUserId);

        // Act - carregar a transação rastreada, atualizar via método de domínio e salvar via SaveChangesAsync
        await using (AppDbContext updateContext = fixture.CreateContext(userContext: updateUserContext, tenantContext: tenantContext))
        {
            var updateRepo = new TransactionRepository(context: updateContext);
            Transaction? txToUpdate = await updateRepo.GetByIdAsync(id: transactionId, cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(@object: txToUpdate);

            txToUpdate.Update(
                title: "Prestação Concluída",
                type: ETransactionType.Withdraw,
                amount: 1250.75m,
                categoryId: category2.Id);

            await updateContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);
        }

        // Assert - consultar em novo contexto sem cache e validar campos de negócio e auditoria
        await using AppDbContext verifyContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var verifyRepo = new TransactionRepository(context: verifyContext);
        Transaction? updated = await verifyRepo.GetByIdAsync(id: transactionId, cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(@object: updated);
        Assert.Equal(expected: transactionId, actual: updated.Id);
        Assert.Equal(expected: "Prestação Concluída", actual: updated.Title);
        Assert.Equal(expected: ETransactionType.Withdraw, actual: updated.Type);
        Assert.Equal(expected: 1250.75m, actual: updated.Amount);
        Assert.Equal(expected: category2.Id, actual: updated.CategoryId);
        Assert.Equal(expected: tenantId, actual: updated.TenantId);
        Assert.Equal(expected: userId, actual: updated.CreatedBy);
        Assert.Equal(expected: updateUserId, actual: updated.UpdatedBy);
        Assert.NotNull(@object: updated.UpdatedAt);
    }

    [Fact]
    public async Task ExistsAsync_WhenTransactionExistsForTenant_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        var tenantContext = new TenantContextFake(tenantId: tenantId);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var categoryRepo = new CategoryRepository(context: context);
        var transactionRepo = new TransactionRepository(context: context);

        var category = Category.Create(title: "Serviços", description: "Prestação de serviços");
        await categoryRepo.AddAsync(category: category, cancellationToken: TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        var transaction = Transaction.Create(
            title: "Honorários",
            type: ETransactionType.Deposit,
            amount: 5000.00m,
            categoryId: category.Id);
        await transactionRepo.AddAsync(transaction: transaction, cancellationToken: TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await using AppDbContext verifyContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var verifyRepo = new TransactionRepository(context: verifyContext);
        bool exists = await verifyRepo.ExistsAsync(id: transaction.Id, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.True(condition: exists, userMessage: "A transação existente deve retornar true.");
    }

    [Fact]
    public async Task ExistsAsync_WhenTransactionDoesNotExistOrBelongsToAnotherTenant_ReturnsFalse()
    {
        // Arrange
        var tenant1 = Guid.NewGuid();
        var tenant2 = Guid.NewGuid();
        await using AppDbContext contextTenant1 = fixture.CreateContext(userContext: new UserContextFake(userId: Guid.NewGuid()), tenantContext: new TenantContextFake(tenantId: tenant1));
        var catRepoTenant1 = new CategoryRepository(context: contextTenant1);
        var txRepoTenant1 = new TransactionRepository(context: contextTenant1);

        var category = Category.Create(title: "Investimentos", description: "Aplicações");
        await catRepoTenant1.AddAsync(category: category, cancellationToken: TestContext.Current.CancellationToken);
        await contextTenant1.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        var transaction = Transaction.Create(
            title: "Dividendos",
            type: ETransactionType.Deposit,
            amount: 750.00m,
            categoryId: category.Id);
        await txRepoTenant1.AddAsync(transaction: transaction, cancellationToken: TestContext.Current.CancellationToken);
        await contextTenant1.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act - consultar sob Tenant 2
        await using AppDbContext contextTenant2 = fixture.CreateContext(userContext: new UserContextFake(userId: Guid.NewGuid()), tenantContext: new TenantContextFake(tenantId: tenant2));
        var repoTenant2 = new TransactionRepository(context: contextTenant2);
        bool existsForOtherTenant = await repoTenant2.ExistsAsync(id: transaction.Id, cancellationToken: TestContext.Current.CancellationToken);
        bool existsForRandomId = await repoTenant2.ExistsAsync(id: Guid.NewGuid(), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: existsForOtherTenant, userMessage: "A transação de outro tenant não deve ser encontrada.");
        Assert.False(condition: existsForRandomId, userMessage: "Um id inexistente deve retornar false.");
    }

    [Fact]
    public async Task RemoveAsync_RemovesTransactionFromPostgreSqlDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        var tenantContext = new TenantContextFake(tenantId: tenantId);

        Guid transactionId;

        await using (AppDbContext setupContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext))
        {
            var catRepo = new CategoryRepository(context: setupContext);
            var txRepo = new TransactionRepository(context: setupContext);

            var category = Category.Create(title: "Transporte", description: "Gastos com locomoção");
            await catRepo.AddAsync(category: category, cancellationToken: TestContext.Current.CancellationToken);
            await setupContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

            var transaction = Transaction.Create(
                title: "Combustível",
                type: ETransactionType.Withdraw,
                amount: 180.00m,
                categoryId: category.Id);
            await txRepo.AddAsync(transaction: transaction, cancellationToken: TestContext.Current.CancellationToken);
            await setupContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);
            transactionId = transaction.Id;
        }

        // Act - carregar no repositório, chamar RemoveAsync e salvar alterações
        await using (AppDbContext removeContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext))
        {
            var removeRepo = new TransactionRepository(context: removeContext);
            Transaction? transactionToRemove = await removeRepo.GetByIdAsync(id: transactionId, cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(@object: transactionToRemove);

            await removeRepo.RemoveAsync(transaction: transactionToRemove, cancellationToken: TestContext.Current.CancellationToken);
            await removeContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);
        }

        // Assert - consultar em novo contexto e verificar que não existe mais
        await using AppDbContext verifyContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var verifyRepo = new TransactionRepository(context: verifyContext);
        Transaction? deletedTransaction = await verifyRepo.GetByIdAsync(id: transactionId, cancellationToken: TestContext.Current.CancellationToken);
        bool exists = await verifyRepo.ExistsAsync(id: transactionId, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Null(@object: deletedTransaction);
        Assert.False(condition: exists, userMessage: "A transação removida não deve mais existir.");
    }

    [Fact]
    public async Task ExistsByCategoryIdAsync_WhenTransactionExistsForCategory_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        var tenantContext = new TenantContextFake(tenantId: tenantId);

        Guid categoryId;

        await using (AppDbContext setupContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext))
        {
            var catRepo = new CategoryRepository(context: setupContext);
            var txRepo = new TransactionRepository(context: setupContext);

            var category = Category.Create(title: "Alimentação", description: "Gastos com mercado");
            await catRepo.AddAsync(category: category, cancellationToken: TestContext.Current.CancellationToken);
            await setupContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

            var transaction = Transaction.Create(
                title: "Feira Semanal",
                type: ETransactionType.Withdraw,
                amount: 120.00m,
                categoryId: category.Id);
            await txRepo.AddAsync(transaction: transaction, cancellationToken: TestContext.Current.CancellationToken);
            await setupContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);
            categoryId = category.Id;
        }

        // Act
        await using AppDbContext verifyContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var verifyRepo = new TransactionRepository(context: verifyContext);
        bool exists = await verifyRepo.ExistsByCategoryIdAsync(categoryId: categoryId, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.True(condition: exists, userMessage: "Deve retornar true quando existirem transações vinculadas à categoria.");
    }

    [Fact]
    public async Task ExistsByCategoryIdAsync_WhenNoTransactionForCategoryOrDifferentTenant_ReturnsFalse()
    {
        // Arrange
        var tenant1 = Guid.NewGuid();
        var tenant2 = Guid.NewGuid();

        Guid categoryIdWithTx;
        Guid categoryIdWithoutTx;

        await using (AppDbContext contextTenant1 = fixture.CreateContext(userContext: new UserContextFake(userId: Guid.NewGuid()), tenantContext: new TenantContextFake(tenantId: tenant1)))
        {
            var catRepoTenant1 = new CategoryRepository(context: contextTenant1);
            var txRepoTenant1 = new TransactionRepository(context: contextTenant1);

            var catWithTx = Category.Create(title: "Com Transação", description: "Desc");
            var catWithoutTx = Category.Create(title: "Sem Transação", description: "Desc");
            await catRepoTenant1.AddAsync(category: catWithTx, cancellationToken: TestContext.Current.CancellationToken);
            await catRepoTenant1.AddAsync(category: catWithoutTx, cancellationToken: TestContext.Current.CancellationToken);
            await contextTenant1.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

            var transaction = Transaction.Create(
                title: "Despesa",
                type: ETransactionType.Withdraw,
                amount: 90.00m,
                categoryId: catWithTx.Id);
            await txRepoTenant1.AddAsync(transaction: transaction, cancellationToken: TestContext.Current.CancellationToken);
            await contextTenant1.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

            categoryIdWithTx = catWithTx.Id;
            categoryIdWithoutTx = catWithoutTx.Id;
        }

        // Act & Assert 1: categoria sem transações sob Tenant 1
        await using (AppDbContext contextTenant1Verify = fixture.CreateContext(userContext: new UserContextFake(userId: Guid.NewGuid()), tenantContext: new TenantContextFake(tenantId: tenant1)))
        {
            var repoTenant1 = new TransactionRepository(context: contextTenant1Verify);
            bool existsWithoutTx = await repoTenant1.ExistsByCategoryIdAsync(categoryId: categoryIdWithoutTx, cancellationToken: TestContext.Current.CancellationToken);
            Assert.False(condition: existsWithoutTx, userMessage: "Não deve encontrar transações para categoria vazia.");
        }

        // Act & Assert 2: categoria com transação sob Tenant 1 consultada por Tenant 2 (isolamento multi-tenant)
        await using AppDbContext contextTenant2Verify = fixture.CreateContext(userContext: new UserContextFake(userId: Guid.NewGuid()), tenantContext: new TenantContextFake(tenantId: tenant2));
        var repoTenant2 = new TransactionRepository(context: contextTenant2Verify);
        bool existsForOtherTenant = await repoTenant2.ExistsByCategoryIdAsync(categoryId: categoryIdWithTx, cancellationToken: TestContext.Current.CancellationToken);
        bool existsForRandomId = await repoTenant2.ExistsByCategoryIdAsync(categoryId: Guid.NewGuid(), cancellationToken: TestContext.Current.CancellationToken);

        Assert.False(condition: existsForOtherTenant, userMessage: "Não deve encontrar transações vinculadas sob outro tenant.");
        Assert.False(condition: existsForRandomId, userMessage: "Um id aleatório de categoria deve retornar false.");
    }
}
