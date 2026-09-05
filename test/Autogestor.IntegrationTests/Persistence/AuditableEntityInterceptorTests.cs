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
            description: "Restaurantes",
            userId: userId);
        context.Categories.Add(entity: category);
        context.SaveChanges();

        Assert.NotEqual(expected: default, actual: category.CreatedAt);
        Assert.NotEqual(expected: default, actual: category.UpdatedAt);
        Assert.Equal(expected: userId, actual: category.CreatedBy);
        Assert.Equal(expected: userId, actual: category.UpdatedBy);
    }

    [Fact]
    public async Task SavingChangesAsync_WhenCategoryIsAdded_PopulatesAuditFields()
    {
        var userId = Guid.NewGuid();
        var userContext = new UserContextFake(userId: userId);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext);

        var category = Category.Create(
            title: "Transporte",
            description: "Combustível",
            userId: userId);
        await context.Categories.AddAsync(entity: category);
        await context.SaveChangesAsync();

        Assert.NotEqual(expected: default, actual: category.CreatedAt);
        Assert.NotEqual(expected: default, actual: category.UpdatedAt);
        Assert.Equal(expected: userId, actual: category.CreatedBy);
        Assert.Equal(expected: userId, actual: category.UpdatedBy);
    }

    [Fact]
    public async Task SavingChanges_WhenCategoryIsModified_UpdatesUpdatedAtAndUpdatedBy()
    {
        var initialUser = Guid.NewGuid();
        var userContext = new UserContextFake(userId: initialUser);
        await using AppDbContext context = fixture.CreateContext(userContext: userContext);

        var category = Category.Create(
            title: "Saúde",
            description: "Remédios",
            userId: initialUser);
        await context.Categories.AddAsync(entity: category);
        await context.SaveChangesAsync();

        DateTime createdAt = category.CreatedAt;

        // Modifica com outro usuário no contexto
        var updatingUser = Guid.NewGuid();
        userContext.UserId = updatingUser;

        context.Entry(entity: category).State = EntityState.Modified;
        await context.SaveChangesAsync();

        Assert.Equal(expected: createdAt, actual: category.CreatedAt);
        Assert.Equal(expected: initialUser, actual: category.CreatedBy);
        Assert.Equal(expected: updatingUser, actual: category.UpdatedBy);
        Assert.True(condition: category.UpdatedAt >= createdAt);
    }
}
