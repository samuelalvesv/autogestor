using Autogestor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autogestor.Infrastructure.Persistence.Configurations;

public abstract class AuditableEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : AuditableEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Configure standard base primary key inherited from Entity
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedNever() // GUID is generated in C# constructor
            .HasColumnType("uuid");

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

        builder.Property(e => e.UpdatedBy)
            .IsRequired()
            .HasColumnType("uuid");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasColumnType("timestamptz");
    }
}
