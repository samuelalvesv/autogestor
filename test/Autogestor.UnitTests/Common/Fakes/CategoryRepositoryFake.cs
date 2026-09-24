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
    public int? LastPagedSkip { get; private set; }
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

    public Task<(IReadOnlyList<Category> categories, int count)> GetPagedAsync(
        int skip,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        PassedCancellationToken = cancellationToken;
        LastPagedSkip = skip;
        LastPagedPageSize = pageSize;

        if (_categories.Count == 0 || skip >= _categories.Count)
        {
            return Task.FromResult<(IReadOnlyList<Category> categories, int count)>(
                result: (categories: [], count: _categories.Count));
        }

        IReadOnlyList<Category> result = _categories
            .Skip(count: skip)
            .Take(count: pageSize)
            .ToList()
            .AsReadOnly();

        return Task.FromResult(
            result: (categories: result, count: _categories.Count));
    }
}
