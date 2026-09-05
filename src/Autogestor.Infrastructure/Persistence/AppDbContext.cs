using Autogestor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Autogestor.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options: options)
{
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(assembly: typeof(AppDbContext).Assembly);
}
