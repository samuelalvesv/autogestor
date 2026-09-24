using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Autogestor.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public void Add(Category category)
        => context.Categories.Add(entity: category);

    public void Remove(Category category)
        => context.Categories.Remove(entity: category);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Categories
            .AnyAsync(
                predicate: c => c.Id == id,
                cancellationToken: cancellationToken);

    public Task<Category?> GetByIdAsync(
        Guid id,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Category> query = context.Categories;

        if (asNoTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefaultAsync(
            predicate: c => c.Id == id,
            cancellationToken: cancellationToken);
    }

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
