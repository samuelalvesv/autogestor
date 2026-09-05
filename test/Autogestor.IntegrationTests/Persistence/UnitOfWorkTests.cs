using Autogestor.Domain.Entities;
using Autogestor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Autogestor.IntegrationTests.Persistence;

public class UnitOfWorkTests
{
    private static AppDbContext CreateContext()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString: "Host=localhost;Database=test;Username=postgres;Password=postgres")
            .Options;

        return new AppDbContext(options: options);
    }

    [Fact]
    public async Task CommitAsync_WithoutChanges_ReturnsZero()
    {
        await using AppDbContext context = CreateContext();
        var unitOfWork = new UnitOfWork(context: context);

        int result = await unitOfWork.CommitAsync();

        Assert.Equal(expected: 0, actual: result);
    }

    [Fact]
    public async Task CommitAsync_WithChangesAndCancelledToken_ThrowsOperationCanceledException()
    {
        await using AppDbContext context = CreateContext();
        var unitOfWork = new UnitOfWork(context: context);
        var category = Category.Create(
            title: "Test",
            description: "Description",
            userId: Guid.NewGuid());
        context.Categories.Add(entity: category);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            testCode: () => unitOfWork.CommitAsync(cancellationToken: cts.Token));
    }
}
