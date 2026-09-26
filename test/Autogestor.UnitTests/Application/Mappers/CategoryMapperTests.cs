using Autogestor.Application.Mappers;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.UnitTests.Common.Fakes;

namespace Autogestor.UnitTests.Application.Mappers;

public sealed class CategoryMapperTests
{
    [Fact]
    public void ToResponse_WithValidCategory_MapsAllFieldsCorrectly()
    {
        // Arrange
        var category = Category.Create(
            title: "Alimentação",
            description: "Restaurantes e supermercados");

        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        DateTime createdAt = DateTime.UtcNow;
        var updatedBy = Guid.NewGuid();
        DateTime updatedAt = DateTime.UtcNow.AddMinutes(value: 5);

        EntityPersistenceHelper.SetPersistenceFields(
            entity: category,
            userId: userId,
            tenantId: tenantId,
            timestamp: createdAt);

        EntityPersistenceHelper.SetAuditUpdateFields(
            entity: category,
            updatedBy: updatedBy,
            updatedAt: updatedAt);

        // Act
        CategoryResponse response = CategoryMapper.ToResponse(category: category);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Equal(expected: category.Id, actual: response.Id);
        Assert.Equal(expected: category.Title, actual: response.Title);
        Assert.Equal(expected: category.Description, actual: response.Description);
        Assert.Equal(expected: category.Active, actual: response.Active);
        Assert.Equal(expected: userId, actual: response.CreatedBy);
        Assert.Equal(expected: createdAt, actual: response.CreatedAt);
        Assert.Equal(expected: updatedBy, actual: response.UpdatedBy);
        Assert.Equal(expected: updatedAt, actual: response.UpdatedAt);
        Assert.Equal(expected: tenantId, actual: response.TenantId);
    }

    [Fact]
    public void ToResponseList_WithListOfCategories_MapsAllItemsCorrectly()
    {
        // Arrange
        var category1 = Category.Create(title: "Categoria 1", description: "Desc 1");
        var category2 = Category.Create(title: "Categoria 2", description: "Desc 2");
        IReadOnlyList<Category> categories = [category1, category2];

        // Act
        IReadOnlyList<CategoryResponse> responses = CategoryMapper.ToResponseList(categories: categories);

        // Assert
        Assert.NotNull(@object: responses);
        Assert.Equal(expected: 2, actual: responses.Count);
        Assert.Equal(expected: category1.Id, actual: responses[0].Id);
        Assert.Equal(expected: category1.Title, actual: responses[0].Title);
        Assert.Equal(expected: category2.Id, actual: responses[1].Id);
        Assert.Equal(expected: category2.Title, actual: responses[1].Title);
    }

    [Fact]
    public void ToPagedResponse_WithItemsAndHasNextPageTrue_ReturnsNextCursorAsLastItemId()
    {
        // Arrange
        var category1 = Category.Create(title: "Categoria 1", description: "Desc 1");
        var category2 = Category.Create(title: "Categoria 2", description: "Desc 2");
        IReadOnlyList<Category> categories = [category1, category2];

        // Act
        var response = CategoryMapper.ToPagedResponse(
            categories: categories,
            hasNextPage: true);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Equal(expected: 2, actual: response.Data.Count);
        Assert.True(condition: response.HasNextPage);
        Assert.Equal(expected: category2.Id, actual: response.NextCursor);
    }

    [Fact]
    public void ToPagedResponse_WithItemsAndHasNextPageFalse_ReturnsNullNextCursor()
    {
        // Arrange
        var category1 = Category.Create(title: "Categoria 1", description: "Desc 1");
        IReadOnlyList<Category> categories = [category1];

        // Act
        var response = CategoryMapper.ToPagedResponse(
            categories: categories,
            hasNextPage: false);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Single(collection: response.Data);
        Assert.False(condition: response.HasNextPage);
        Assert.Null(@object: response.NextCursor);
    }

    [Fact]
    public void ToPagedResponse_WithEmptyOrNullList_ReturnsEmptyDataAndNullNextCursor()
    {
        // Act - Empty
        var emptyResponse = CategoryMapper.ToPagedResponse(
            categories: [],
            hasNextPage: true);

        // Assert - Empty
        Assert.Empty(collection: emptyResponse.Data);
        Assert.True(condition: emptyResponse.HasNextPage);
        Assert.Null(@object: emptyResponse.NextCursor);

        // Act - Null
        var nullResponse = CategoryMapper.ToPagedResponse(
            categories: null,
            hasNextPage: false);

        // Assert - Null
        Assert.Empty(collection: nullResponse.Data);
        Assert.False(condition: nullResponse.HasNextPage);
        Assert.Null(@object: nullResponse.NextCursor);
    }
}
