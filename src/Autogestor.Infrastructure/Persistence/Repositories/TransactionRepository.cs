using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Autogestor.Infrastructure.Persistence.Repositories;

public sealed class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        context.Transactions.Add(entity: transaction);
        return Task.CompletedTask;
    }

    public Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                predicate: t => t.Id == id,
                cancellationToken: cancellationToken);

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
