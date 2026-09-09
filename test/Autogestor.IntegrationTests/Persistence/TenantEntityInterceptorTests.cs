using Autogestor.Domain.Entities;
using Autogestor.Infrastructure.Persistence;
using Autogestor.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Autogestor.IntegrationTests.Persistence;

[Collection(name: "PostgreSql")]
public class TenantEntityInterceptorTests(PostgreSqlFixture fixture)
{
    [Fact]
    public async Task SavingChangesAsync_WhenTenantEntityIsAdded_PopulatesTenantId()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenantContext = new TenantContextFake(tenantId: tenantId);
        await using AppDbContext context = fixture.CreateContext(tenantContext: tenantContext);

        var category = Category.Create(
            title: "Eletrônicos",
            description: "Gadgets e informática");

        // Act
        await context.Categories.AddAsync(entity: category);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(expected: tenantId, actual: category.TenantId);

        await using AppDbContext verifyContext = fixture.CreateContext(tenantContext: tenantContext);
        Category? persisted = await verifyContext.Categories.FirstOrDefaultAsync(predicate: c => c.Id == category.Id);
        Assert.NotNull(@object: persisted);
        Assert.Equal(expected: tenantId, actual: persisted.TenantId);
    }

    [Fact]
    public async Task SavingChangesAsync_WhenTenantEntityIsModified_PreventsTenantIdFromChanging()
    {
        // Arrange
        var originalTenantId = Guid.NewGuid();
        var tenantContext = new TenantContextFake(tenantId: originalTenantId);
        await using AppDbContext context = fixture.CreateContext(tenantContext: tenantContext);

        var category = Category.Create(
            title: "Móveis",
            description: "Mobiliário para escritório");

        await context.Categories.AddAsync(entity: category);
        await context.SaveChangesAsync();

        // Act - Tenta alterar o TenantId diretamente via Entry
        var maliciousTenantId = Guid.NewGuid();
        context.Entry(entity: category).Property(propertyExpression: c => c.TenantId).CurrentValue = maliciousTenantId;
        context.Entry(entity: category).State = EntityState.Modified;
        await context.SaveChangesAsync();

        // Assert - O interceptador deve ter marcado IsModified = false para o TenantId
        await using AppDbContext verifyContext = fixture.CreateContext(tenantContext: tenantContext);
        Category? persisted = await verifyContext.Categories.FirstOrDefaultAsync(predicate: c => c.Id == category.Id);
        Assert.NotNull(@object: persisted);
        Assert.Equal(expected: originalTenantId, actual: persisted.TenantId);
    }
}
