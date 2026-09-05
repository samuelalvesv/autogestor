using Autogestor.Domain.Entities;
using Autogestor.Infrastructure.Persistence;
using Autogestor.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Autogestor.IntegrationTests.Persistence;

[Collection(name: "PostgreSql")]
public class UnitOfWorkTests(PostgreSqlFixture fixture)
{
    [Fact]
    public async Task CommitAsync_WithoutChanges_ReturnsZero()
    {
        await using AppDbContext context = fixture.CreateContext();
        var unitOfWork = new UnitOfWork(context: context);

        int result = await unitOfWork.CommitAsync();

        Assert.Equal(expected: 0, actual: result);
    }

    [Fact]
    public async Task CommitAsync_WithAddedEntity_PersistsChangesAndReturnsAffectedCount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext);
        var unitOfWork = new UnitOfWork(context: context);

        var category = Category.Create(
            title: "UnitOfWork Test",
            description: "Testando commit real no banco",
            userId: userId);
        await context.Categories.AddAsync(entity: category);

        // Act
        int affectedRows = await unitOfWork.CommitAsync();

        // Assert
        Assert.Equal(expected: 1, actual: affectedRows);

        await using AppDbContext verifyContext = fixture.CreateContext();
        Category? persisted = await verifyContext.Categories.AsNoTracking().FirstOrDefaultAsync(
            predicate: c => c.Id == category.Id);

        Assert.NotNull(@object: persisted);
        Assert.Equal(expected: "UnitOfWork Test", actual: persisted.Title);
        Assert.Equal(expected: userId, actual: persisted.CreatedBy);
    }

    [Fact]
    public async Task CommitAsync_WithChangesAndCancelledToken_ThrowsOperationCanceledException()
    {
        await using AppDbContext context = fixture.CreateContext();
        var unitOfWork = new UnitOfWork(context: context);
        var category = Category.Create(
            title: "Test",
            description: "Description",
            userId: Guid.NewGuid());
        await context.Categories.AddAsync(entity: category);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            testCode: () => unitOfWork.CommitAsync(cancellationToken: cts.Token));
    }
}
