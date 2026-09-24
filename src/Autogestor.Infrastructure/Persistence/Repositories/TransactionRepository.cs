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

    public async Task<IReadOnlyList<Transaction>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await context.Transactions
            .AsNoTracking()
            .OrderByDescending(keySelector: t => t.CreatedAt)
            .Skip(count: (pageNumber - 1) * pageSize)
            .Take(count: pageSize)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}
