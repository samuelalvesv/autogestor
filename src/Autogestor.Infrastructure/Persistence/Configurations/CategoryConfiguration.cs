using Autogestor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autogestor.Infrastructure.Persistence.Configurations;

public sealed class CategoryConfiguration : TenantEntityConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder: builder);

        builder.Property(propertyExpression: c => c.Title)
            .IsRequired()
            .HasColumnType(typeName: "text");

        builder.Property(propertyExpression: c => c.Description)
            .IsRequired()
            .HasColumnType(typeName: "text");
    }
}
