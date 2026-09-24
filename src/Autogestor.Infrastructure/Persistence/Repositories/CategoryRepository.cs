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

    public async Task<(IReadOnlyList<Category> categories, int count)> GetPagedAsync(
        int skip,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Category> query = context.Categories.AsNoTracking();

        int count = await query.CountAsync(cancellationToken: cancellationToken);
        if (count == 0 || skip >= count)
            return ([], count);

        IReadOnlyList<Category> categories = await query
            .OrderByDescending(keySelector: c => c.CreatedAt)
            .Skip(count: skip)
            .Take(count: pageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        return (categories, count);
    }
}
