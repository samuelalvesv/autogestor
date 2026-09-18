using Autogestor.Domain.Entities;

namespace Autogestor.Domain.Interfaces;

public interface ICategoryRepository
{
    Task CreateAsync(Category category, CancellationToken cancellationToken = default);
    Task UpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
