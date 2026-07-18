using Autogestor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autogestor.Infrastructure.Persistence.Configurations;

public abstract class TenantEntityConfiguration<TEntity> : AuditableEntityConfiguration<TEntity>
    where TEntity : TenantEntity
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.TenantId)
            .IsRequired()
            .HasColumnType("uuid");
    }
}
