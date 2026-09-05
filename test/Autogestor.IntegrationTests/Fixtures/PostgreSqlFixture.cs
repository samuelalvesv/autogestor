using Autogestor.Domain.Interfaces;
using Autogestor.Infrastructure.Persistence;
using Autogestor.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Autogestor.IntegrationTests.Fixtures;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder(image: "postgres:17-alpine")
        .WithDatabase(database: "autogestor_test")
        .WithUsername(username: "postgres")
        .WithPassword(password: "postgres")
        .Build();

    private string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using AppDbContext context = CreateContext();
        await context.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    public AppDbContext CreateContext(IUserContext? userContext = null)
    {
        IUserContext effectiveUserContext = userContext ?? new UserContextFake();
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString: ConnectionString)
            .AddInterceptors(interceptors: new AuditableEntityInterceptor(userContext: effectiveUserContext))
            .Options;

        return new AppDbContext(options: options);
    }
}

[CollectionDefinition(name: "PostgreSql")]
public sealed class PostgreSqlCollection : ICollectionFixture<PostgreSqlFixture>;
