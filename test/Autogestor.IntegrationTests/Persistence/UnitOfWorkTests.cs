using Autogestor.Domain.Entities;
using Autogestor.Infrastructure.Persistence;
using Autogestor.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Autogestor.IntegrationTests.Persistence;

[Collection(name: "PostgreSql")]
public class UnitOfWorkTests(PostgreSqlFixture fixture)
{
    [Fact]
    public async Task CommitAsync_WithoutChanges_CompletesSuccessfully()
    {
        await using AppDbContext context = fixture.CreateContext();
        var unitOfWork = new UnitOfWork(context: context);

        await unitOfWork.CommitAsync();
    }

    [Fact]
    public async Task CommitAsync_WithAddedEntity_PersistsChangesToDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        var tenantContext = new TenantContextFake(tenantId: tenantId);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        var unitOfWork = new UnitOfWork(context: context);

        var category = Category.Create(
            title: "UnitOfWork Test",
            description: "Testando commit real no banco");
        await context.Categories.AddAsync(entity: category);

        // Act
        await unitOfWork.CommitAsync();

        // Assert
        await using AppDbContext verifyContext = fixture.CreateContext(userContext: userContext, tenantContext: tenantContext);
        Category? persisted = await verifyContext.Categories.AsNoTracking().FirstOrDefaultAsync(
            predicate: c => c.Id == category.Id);

        Assert.NotNull(@object: persisted);
        Assert.Equal(expected: "UnitOfWork Test", actual: persisted.Title);
        Assert.Equal(expected: tenantId, actual: persisted.TenantId);
        Assert.Equal(expected: userId, actual: persisted.CreatedBy);
    }

    [Fact]
    public async Task CommitAsync_WithChangesAndCancelledToken_ThrowsOperationCanceledException()
    {
        await using AppDbContext context = fixture.CreateContext();
        var unitOfWork = new UnitOfWork(context: context);
        var category = Category.Create(
            title: "Test",
            description: "Description");
        await context.Categories.AddAsync(entity: category);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            testCode: () => unitOfWork.CommitAsync(cancellationToken: cts.Token));
    }
}
