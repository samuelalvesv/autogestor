using Autogestor.Application.Interfaces;

namespace Autogestor.UnitTests.Common.Fakes;

public sealed class UnitOfWorkFake : IUnitOfWork
{
    public int CommitCount { get; private set; }
    public CancellationToken PassedCancellationToken { get; private set; }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        PassedCancellationToken = cancellationToken;
        CommitCount++;
        return Task.CompletedTask;
    }
}
