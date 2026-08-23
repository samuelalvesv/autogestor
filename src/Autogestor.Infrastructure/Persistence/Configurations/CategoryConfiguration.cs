using Autogestor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autogestor.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : AuditableEntityConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(c => c.Description)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(c => c.UserId)
            .IsRequired()
            .HasColumnType("uuid");
    }
}
