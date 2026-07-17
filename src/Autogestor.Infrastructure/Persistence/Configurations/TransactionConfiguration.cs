using Autogestor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autogestor.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : AuditableEntityConfiguration<Transaction>
{
    public override void Configure(EntityTypeBuilder<Transaction> builder)
    {
        base.Configure(builder);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<int>()
            .HasColumnType("integer");

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(t => t.CategoryId)
            .IsRequired()
            .HasColumnType("uuid");

        // relationships
        builder.HasOne(t => t.Category)
            .WithMany()
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}