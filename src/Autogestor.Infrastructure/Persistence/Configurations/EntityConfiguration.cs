using Autogestor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autogestor.Infrastructure.Persistence.Configurations;

public abstract class EntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Configure standard base primary key inherited from Entity
        builder.HasKey(keyExpression: e => e.Id);
        builder.Property(propertyExpression: e => e.Id)
            .ValueGeneratedNever() // UUIDv7 is generated in C# constructor
            .HasColumnType(typeName: "uuid");
    }
}
