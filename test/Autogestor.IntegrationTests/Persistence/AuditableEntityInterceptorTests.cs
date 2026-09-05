using Autogestor.Domain.Entities;
using Autogestor.Infrastructure.Persistence;
using Autogestor.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Autogestor.IntegrationTests.Persistence;

public class AuditableEntityInterceptorTests
{
    private static AppDbContext CreateContext()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString: "Host=localhost;Database=test;Username=postgres;Password=postgres")
            .Options;

        return new AppDbContext(options: options);
    }

    [Fact]
    public void SavingChanges_WhenCategoryIsAdded_PopulatesAuditFields()
    {
        using AppDbContext context = CreateContext();
        var interceptor = new AuditableEntityInterceptor();

        var userId = Guid.NewGuid();
        var category = Category.Create(
            title: "Alimentação",
            description: "Restaurantes",
            userId: userId);
        context.Categories.Add(entity: category);

        var eventData = new DbContextEventData(
            eventDefinition: null!,
            messageGenerator: (_, _) => "Test",
            context: context);

        interceptor.SavingChanges(eventData: eventData, result: default);

        Assert.NotEqual(expected: default, actual: category.CreatedAt);
        Assert.NotEqual(expected: default, actual: category.UpdatedAt);
        Assert.Equal(expected: userId, actual: category.CreatedBy);
        Assert.Equal(expected: userId, actual: category.UpdatedBy);
    }

    [Fact]
    public async Task SavingChangesAsync_WhenCategoryIsAdded_PopulatesAuditFields()
    {
        using AppDbContext context = CreateContext();
        var interceptor = new AuditableEntityInterceptor();

        var userId = Guid.NewGuid();
        var category = Category.Create(
            title: "Transporte",
            description: "Combustível",
            userId: userId);
        context.Categories.Add(entity: category);

        var eventData = new DbContextEventData(
            eventDefinition: null!,
            messageGenerator: (_, _) => "Test",
            context: context);

        await interceptor.SavingChangesAsync(eventData: eventData, result: default);

        Assert.NotEqual(expected: default, actual: category.CreatedAt);
        Assert.NotEqual(expected: default, actual: category.UpdatedAt);
        Assert.Equal(expected: userId, actual: category.CreatedBy);
        Assert.Equal(expected: userId, actual: category.UpdatedBy);
    }

    [Fact]
    public void SavingChanges_WhenCategoryIsModified_UpdatesUpdatedAt()
    {
        using AppDbContext context = CreateContext();
        var interceptor = new AuditableEntityInterceptor();

        var userId = Guid.NewGuid();
        var category = Category.Create(
            title: "Saúde",
            description: "Remédios",
            userId: userId);
        context.Categories.Attach(entity: category);
        context.Entry(entity: category).State = EntityState.Modified;

        var eventData = new DbContextEventData(
            eventDefinition: null!,
            messageGenerator: (_, _) => "Test",
            context: context);

        interceptor.SavingChanges(eventData: eventData, result: default);

        Assert.NotEqual(expected: default, actual: category.UpdatedAt);
    }

    [Fact]
    public void SavingChanges_WithNullContext_DoesNotThrow()
    {
        var interceptor = new AuditableEntityInterceptor();
        var eventData = new DbContextEventData(
            eventDefinition: null!,
            messageGenerator: (_, _) => "Test",
            context: null!);

        Exception? exception = Record.Exception(testCode: () => interceptor.SavingChanges(eventData: eventData, result: default));
        Assert.Null(@object: exception);
    }
}
