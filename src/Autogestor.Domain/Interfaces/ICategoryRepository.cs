using Autogestor.Domain.Entities;

namespace Autogestor.Domain.Interfaces;

public interface ICategoryRepository
{
    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);
}
