using Autogestor.Domain.Entities;
using Autogestor.Infrastructure.Persistence;
using Autogestor.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Autogestor.IntegrationTests.Persistence;

public class CategoryRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString: "Host=localhost;Database=test;Username=postgres;Password=postgres")
            .Options;

        return new AppDbContext(options: options);
    }

    [Fact]
    public async Task AddAsync_AddsCategoryToDbContext()
    {
        using AppDbContext context = CreateContext();
        var repository = new CategoryRepository(context: context);
        var category = Category.Create(
            title: "Test",
            description: "Description",
            userId: Guid.NewGuid());

        await repository.AddAsync(category: category);

        Assert.Contains(expected: category, collection: context.Categories.Local);
    }

    [Fact]
    public async Task GetByIdAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        using AppDbContext context = CreateContext();
        var repository = new CategoryRepository(context: context);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            testCode: () => repository.GetByIdAsync(id: Guid.NewGuid(), cancellationToken: cts.Token));
    }

    [Fact]
    public async Task GetAllAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        using AppDbContext context = CreateContext();
        var repository = new CategoryRepository(context: context);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            testCode: () => repository.GetAllAsync(cancellationToken: cts.Token));
    }
}
