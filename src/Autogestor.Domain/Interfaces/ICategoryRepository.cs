using Autogestor.Domain.Entities;

namespace Autogestor.Domain.Interfaces;

public interface ICategoryRepository
{
    void Add(Category category);
    void Remove(Category category);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(Guid id, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Category> categories, int count)> GetPagedAsync(int skip, int pageSize, CancellationToken cancellationToken = default);
}
