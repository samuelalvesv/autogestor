using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Autogestor.Infrastructure.Persistence.Repositories;

public sealed class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public void Add(Transaction transaction)
        => context.Transactions.Add(entity: transaction);

    public void Remove(Transaction transaction)
        => context.Transactions.Remove(entity: transaction);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Transactions
            .AnyAsync(
                predicate: t => t.Id == id,
                cancellationToken: cancellationToken);

    public Task<bool> ExistsByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default) =>
        context.Transactions
            .AnyAsync(
                predicate: t => t.CategoryId == categoryId,
                cancellationToken: cancellationToken);

    public Task<Transaction?> GetByIdAsync(
        Guid id,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Transaction> query = context.Transactions;

        if (asNoTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefaultAsync(
            predicate: t => t.Id == id,
            cancellationToken: cancellationToken);
    }

    public async Task<(IReadOnlyList<Transaction> Items, bool HasNextPage)> GetPagedAsync(
        Guid? cursor,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Transaction> query = context.Transactions.AsNoTracking();

        if (cursor is not null)
            query = query.Where(predicate: t => t.Id.CompareTo(cursor.Value) < 0);

        List<Transaction> items = await query
            .OrderByDescending(keySelector: t => t.Id)
            .Take(count: pageSize + 1)
            .ToListAsync(cancellationToken: cancellationToken);

        bool hasNextPage = items.Count > pageSize;

        if (hasNextPage)
            items.RemoveAt(index: items.Count - 1);

        return (items, hasNextPage);
    }
}
