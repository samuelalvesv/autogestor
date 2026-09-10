using Autogestor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autogestor.Infrastructure.Persistence.Configurations;

public abstract class AuditableEntityConfiguration<TEntity> : EntityConfiguration<TEntity>
    where TEntity : AuditableEntity
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        // Configure AuditableEntity standard properties
        builder.Property(e => e.Active)
            .IsRequired()
            .HasColumnType("boolean");

        builder.Property(e => e.CreatedBy)
            .IsRequired()
            .HasColumnType("uuid");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamptz");

        builder.Property(propertyExpression: e => e.UpdatedBy)
            .IsRequired(required: false)
            .HasColumnType(typeName: "uuid");

        builder.Property(propertyExpression: e => e.UpdatedAt)
            .IsRequired(required: false)
            .HasColumnType(typeName: "timestamptz");
    }
}
