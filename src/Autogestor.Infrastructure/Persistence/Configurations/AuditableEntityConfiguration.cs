using Autogestor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autogestor.Infrastructure.Persistence.Configurations;

public abstract class AuditableEntityConfiguration<TEntity> : EntityConfiguration<TEntity>
    where TEntity : AuditableEntity
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder: builder);

        // Configure AuditableEntity standard properties
        builder.Property(propertyExpression: e => e.Active)
            .IsRequired()
            .HasColumnType(typeName: "boolean");

        builder.Property(propertyExpression: e => e.CreatedBy)
            .IsRequired()
            .HasColumnType(typeName: "uuid");

        builder.Property(propertyExpression: e => e.CreatedAt)
            .IsRequired()
            .HasColumnType(typeName: "timestamptz");

        builder.Property(propertyExpression: e => e.UpdatedBy)
            .IsRequired(required: false)
            .HasColumnType(typeName: "uuid");

        builder.Property(propertyExpression: e => e.UpdatedAt)
            .IsRequired(required: false)
            .HasColumnType(typeName: "timestamptz");
    }
}
