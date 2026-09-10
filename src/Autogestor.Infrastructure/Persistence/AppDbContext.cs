using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;
using Autogestor.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Autogestor.Infrastructure.Persistence;

public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    ITenantContext tenantContext) : DbContext(options: options)
{
    public Guid CurrentTenantId => tenantContext.TenantId;

    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder: modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(assembly: typeof(AppDbContext).Assembly);
        modelBuilder.ApplyTenantFilters(context: this);
    }
}
