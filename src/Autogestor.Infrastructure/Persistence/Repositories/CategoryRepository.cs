using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Autogestor.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        context.Categories.Add(entity: category);
        return Task.CompletedTask;
    }

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(
                predicate: c => c.Id == id,
                cancellationToken: cancellationToken);

    public async Task<IReadOnlyList<Category>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .AsNoTracking()
            .OrderByDescending(keySelector: c => c.CreatedAt)
            .Skip(count: (pageNumber - 1) * pageSize)
            .Take(count: pageSize)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}
