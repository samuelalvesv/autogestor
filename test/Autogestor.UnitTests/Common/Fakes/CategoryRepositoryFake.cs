using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;

namespace Autogestor.UnitTests.Common.Fakes;

public sealed class CategoryRepositoryFake : ICategoryRepository
{
    private readonly List<Category> _categories = [];

    public IReadOnlyList<Category> Categories => _categories;
    public CancellationToken PassedCancellationToken { get; private set; }

    public Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        PassedCancellationToken = cancellationToken;
        EntityPersistenceHelper.SetPersistenceFields(
            entity: category,
            userId: Guid.NewGuid(),
            tenantId: Guid.NewGuid(),
            timestamp: DateTime.UtcNow);
        _categories.Add(item: category);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Category category, CancellationToken cancellationToken = default)
    {
        PassedCancellationToken = cancellationToken;
        if (!_categories.Remove(item: category))
        {
            _categories.RemoveAll(match: c => c.Id == category.Id);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        PassedCancellationToken = cancellationToken;
        return Task.FromResult(result: _categories.Any(predicate: c => c.Id == id));
    }

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        PassedCancellationToken = cancellationToken;
        return Task.FromResult(result: _categories.FirstOrDefault(predicate: c => c.Id == id));
    }

    public Task<IReadOnlyList<Category>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        PassedCancellationToken = cancellationToken;
        return Task.FromResult<IReadOnlyList<Category>>(
            result: _categories.Skip(count: (pageNumber - 1) * pageSize).Take(count: pageSize).ToList().AsReadOnly());
    }
}
