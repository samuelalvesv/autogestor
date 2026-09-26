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
    public Guid? LastPagedCursor { get; private set; }
    public int? LastPagedPageSize { get; private set; }

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

    public Task<(IReadOnlyList<Transaction> Items, bool HasNextPage)> GetPagedAsync(
        Guid? cursor,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        PassedCancellationToken = cancellationToken;
        LastPagedCursor = cursor;
        LastPagedPageSize = pageSize;

        IEnumerable<Transaction> query = _transactions
            .OrderByDescending(keySelector: t => t.Id);

        if (cursor is not null)
            query = query.Where(predicate: t => t.Id.CompareTo(cursor.Value) < 0);

        List<Transaction> items = [.. query.Take(count: pageSize + 1)];

        bool hasNextPage = items.Count > pageSize;
        if (hasNextPage)
        {
            items.RemoveAt(index: items.Count - 1);
        }

        return Task.FromResult(result: ((IReadOnlyList<Transaction>)items, hasNextPage));
    }
}
