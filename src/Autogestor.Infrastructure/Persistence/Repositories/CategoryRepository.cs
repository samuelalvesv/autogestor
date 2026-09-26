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

    public async Task<(IReadOnlyList<Category> Items, bool HasNextPage)> GetPagedAsync(
        Guid? cursor,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Category> query = context.Categories.AsNoTracking();

        if (cursor is not null)
            query = query.Where(predicate: c => c.Id.CompareTo(cursor.Value) < 0);

        List<Category> items = await query
            .OrderByDescending(keySelector: c => c.Id)
            .Take(count: pageSize + 1)
            .ToListAsync(cancellationToken: cancellationToken);

        bool hasNextPage = items.Count > pageSize;

        if (hasNextPage)
            items.RemoveAt(index: items.Count - 1);

        return (items, hasNextPage);
    }
}
