using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;

namespace Autogestor.UnitTests.Common.Fakes;

public sealed class TransactionRepositoryFake : ITransactionRepository
{
    private readonly List<Transaction> _transactions = [];

    public IReadOnlyList<Transaction> Transactions => _transactions;
    public CancellationToken PassedCancellationToken { get; private set; }
    public bool? LastGetByIdAsNoTracking { get; private set; }
    public int ExistsByCategoryIdCallCount { get; private set; }

    public void Add(Transaction transaction)
    {
        EntityPersistenceHelper.SetPersistenceFields(
            entity: transaction,
            userId: Guid.NewGuid(),
            tenantId: Guid.NewGuid(),
            timestamp: DateTime.UtcNow);
        _transactions.Add(item: transaction);
    }

    public void Remove(Transaction transaction)
    {
        if (!_transactions.Remove(item: transaction))
        {
            _transactions.RemoveAll(match: t => t.Id == transaction.Id);
        }
    }

    public Task<bool> ExistsByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        ExistsByCategoryIdCallCount++;
        PassedCancellationToken = cancellationToken;
        return Task.FromResult(result: _transactions.Any(predicate: t => t.CategoryId == categoryId));
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        PassedCancellationToken = cancellationToken;
        return Task.FromResult(result: _transactions.Any(predicate: t => t.Id == id));
    }

    public Task<Transaction?> GetByIdAsync(
        Guid id,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    {
        PassedCancellationToken = cancellationToken;
        LastGetByIdAsNoTracking = asNoTracking;
        return Task.FromResult(result: _transactions.FirstOrDefault(predicate: t => t.Id == id));
    }

    public Task<IReadOnlyList<Transaction>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        PassedCancellationToken = cancellationToken;
        return Task.FromResult<IReadOnlyList<Transaction>>(
            result: _transactions.Skip(count: (pageNumber - 1) * pageSize).Take(count: pageSize).ToList().AsReadOnly());
    }
}
