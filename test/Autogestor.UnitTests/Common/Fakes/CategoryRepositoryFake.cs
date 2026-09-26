using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;

namespace Autogestor.UnitTests.Common.Fakes;

public sealed class CategoryRepositoryFake : ICategoryRepository
{
    private readonly List<Category> _categories = [];

    public IReadOnlyList<Category> Categories => _categories;
    public CancellationToken PassedCancellationToken { get; private set; }
    public bool? LastGetByIdAsNoTracking { get; private set; }
    public int ExistsCallCount { get; private set; }
    public Guid? LastPagedCursor { get; private set; }
    public int? LastPagedPageSize { get; private set; }

    public void Add(Category category)
    {
        EntityPersistenceHelper.SetPersistenceFields(
            entity: category,
            userId: Guid.NewGuid(),
            tenantId: Guid.NewGuid(),
            timestamp: DateTime.UtcNow);
        _categories.Add(item: category);
    }

    public void Remove(Category category)
    {
        if (!_categories.Remove(item: category))
        {
            _categories.RemoveAll(match: c => c.Id == category.Id);
        }
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ExistsCallCount++;
        PassedCancellationToken = cancellationToken;
        return Task.FromResult(result: _categories.Any(predicate: c => c.Id == id));
    }

    public Task<Category?> GetByIdAsync(
        Guid id,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    {
        PassedCancellationToken = cancellationToken;
        LastGetByIdAsNoTracking = asNoTracking;
        return Task.FromResult(result: _categories.FirstOrDefault(predicate: c => c.Id == id));
    }

    public Task<(IReadOnlyList<Category> Items, bool HasNextPage)> GetPagedAsync(
        Guid? cursor,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        PassedCancellationToken = cancellationToken;
        LastPagedCursor = cursor;
        LastPagedPageSize = pageSize;

        IEnumerable<Category> query = _categories
            .OrderByDescending(keySelector: c => c.Id);

        if (cursor is not null)
            query = query.Where(predicate: c => c.Id.CompareTo(cursor.Value) < 0);

        List<Category> items = [.. query.Take(count: pageSize + 1)];

        bool hasNextPage = items.Count > pageSize;
        if (hasNextPage)
        {
            items.RemoveAt(index: items.Count - 1);
        }

        return Task.FromResult(result: ((IReadOnlyList<Category>)items, hasNextPage));
    }
}
