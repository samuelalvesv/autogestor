using Autogestor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autogestor.Infrastructure.Persistence.Configurations;

public sealed class TransactionConfiguration : TenantEntityConfiguration<Transaction>
{
    public override void Configure(EntityTypeBuilder<Transaction> builder)
    {
        base.Configure(builder: builder);

        builder.Property(propertyExpression: t => t.Title)
            .IsRequired()
            .HasColumnType(typeName: "text");

        builder.Property(propertyExpression: t => t.Type)
            .IsRequired()
            .HasConversion<int>()
            .HasColumnType(typeName: "integer");

        builder.Property(propertyExpression: t => t.Amount)
            .IsRequired()
            .HasColumnType(typeName: "numeric(18,2)");

        builder.Property(propertyExpression: t => t.CategoryId)
            .IsRequired()
            .HasColumnType(typeName: "uuid");

        // relationships
        builder.HasOne(navigationExpression: t => t.Category)
            .WithMany()
            .HasForeignKey(foreignKeyExpression: t => t.CategoryId)
            .OnDelete(deleteBehavior: DeleteBehavior.Restrict);
    }
}
