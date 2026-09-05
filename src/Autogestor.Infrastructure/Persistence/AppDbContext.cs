using Autogestor.Domain.Entities;
using Autogestor.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Autogestor.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options: options)
{
    private static readonly AuditableEntityInterceptor AuditableInterceptor = new();

    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder: optionsBuilder);
        optionsBuilder.AddInterceptors(interceptors: AuditableInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(assembly: typeof(AppDbContext).Assembly);
}

