using Autogestor.Domain.Entities;

namespace Autogestor.Domain.Interfaces;

public interface ITransactionRepository
{
    void Add(Transaction transaction);
    void Remove(Transaction transaction);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Transaction?> GetByIdAsync(Guid id, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Transaction>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
