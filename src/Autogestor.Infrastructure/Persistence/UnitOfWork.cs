using Autogestor.Application.Interfaces;

namespace Autogestor.Infrastructure.Persistence;

public sealed class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task CommitAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken: cancellationToken);
}
