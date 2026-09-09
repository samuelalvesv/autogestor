using Autogestor.Domain.Entities;
using Autogestor.Infrastructure.Persistence;
using Autogestor.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Autogestor.IntegrationTests.Persistence;

[Collection(name: "PostgreSql")]
public class AuditableEntityInterceptorTests(PostgreSqlFixture fixture)
{
    [Fact]
    public void SavingChanges_WhenCategoryIsAdded_PopulatesAuditFields()
    {
        var userId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        using AppDbContext context = fixture.CreateContext(userContext: userContext);

        var category = Category.Create(
            title: "Alimentação",
            description: "Restaurantes");
        context.Categories.Add(entity: category);
        context.SaveChanges();

        Assert.NotEqual(expected: default, actual: category.CreatedAt);
        Assert.Null(@object: category.UpdatedAt);
        Assert.Equal(expected: userId, actual: category.CreatedBy);
        Assert.Null(@object: category.UpdatedBy);
    }

    [Fact]
    public async Task SavingChangesAsync_WhenCategoryIsAdded_PopulatesAuditFields()
    {
        var userId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext);

        var category = Category.Create(
            title: "Transporte",
            description: "Combustível");
        await context.Categories.AddAsync(entity: category, cancellationToken: TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotEqual(expected: default, actual: category.CreatedAt);
        Assert.Null(@object: category.UpdatedAt);
        Assert.Equal(expected: userId, actual: category.CreatedBy);
        Assert.Null(@object: category.UpdatedBy);
    }

    [Fact]
    public async Task SavingChanges_WhenCategoryIsModified_UpdatesUpdatedAtAndUpdatedBy()
    {
        var initialUser = Guid.NewGuid();
        var userContext = new UserContextFake(userId: initialUser);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext);

        var category = Category.Create(
            title: "Saúde",
            description: "Remédios");
        await context.Categories.AddAsync(entity: category, cancellationToken: TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        DateTime createdAt = category.CreatedAt;

        // Modifica com outro usuário no contexto e tenta alterar campos imutáveis de criação
        var updatingUser = Guid.NewGuid();
        userContext.UserId = updatingUser;

        context.Entry(entity: category).Property(propertyExpression: c => c.CreatedAt).CurrentValue = DateTime.UtcNow.AddDays(value: -10);
        context.Entry(entity: category).Property(propertyExpression: c => c.CreatedBy).CurrentValue = Guid.NewGuid();
        context.Entry(entity: category).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(expected: createdAt, actual: category.CreatedAt);
        Assert.Equal(expected: initialUser, actual: category.CreatedBy);
        Assert.Equal(expected: updatingUser, actual: category.UpdatedBy);
        Assert.NotNull(@object: category.UpdatedAt);
        Assert.True(condition: category.UpdatedAt >= createdAt);
    }
}
