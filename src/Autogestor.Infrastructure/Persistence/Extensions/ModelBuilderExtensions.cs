using System.Linq.Expressions;
using Autogestor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Autogestor.Infrastructure.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplyTenantFilters(this ModelBuilder modelBuilder, AppDbContext context)
    {
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(TenantEntity).IsAssignableFrom(c: entityType.ClrType))
                continue;

            ParameterExpression parameter = Expression.Parameter(type: entityType.ClrType, name: "e");
            MemberExpression tenantIdProperty = Expression.Property(expression: parameter, propertyName: nameof(TenantEntity.TenantId));
            MemberExpression currentTenantProperty = Expression.Property(
                expression: Expression.Constant(value: context),
                propertyName: nameof(AppDbContext.CurrentTenantId));

            BinaryExpression comparison = Expression.Equal(left: tenantIdProperty, right: currentTenantProperty);
            LambdaExpression filter = Expression.Lambda(body: comparison, parameters: parameter);

            modelBuilder.Entity(type: entityType.ClrType).HasQueryFilter(filter: filter);
        }

        return modelBuilder;
    }
}
