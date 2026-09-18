using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;

namespace Autogestor.UnitTests.Common.Fakes;

public sealed class TransactionRepositoryFake : ITransactionRepository
{
    private readonly List<Transaction> _transactions = [];

    public IReadOnlyList<Transaction> Transactions => _transactions;
    public CancellationToken PassedCancellationToken { get; private set; }

    public Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        PassedCancellationToken = cancellationToken;
        EntityPersistenceHelper.SetPersistenceFields(
            entity: transaction,
            userId: Guid.NewGuid(),
            tenantId: Guid.NewGuid(),
            timestamp: DateTime.UtcNow);
        _transactions.Add(item: transaction);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        PassedCancellationToken = cancellationToken;
        return Task.FromResult(result: _transactions.Any(predicate: t => t.Id == id));
    }

    public Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        PassedCancellationToken = cancellationToken;
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
