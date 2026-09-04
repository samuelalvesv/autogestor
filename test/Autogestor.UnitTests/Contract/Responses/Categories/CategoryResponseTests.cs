using Autogestor.Contract.Responses.Categories;

namespace Autogestor.UnitTests.Contract.Responses.Categories;

public class CategoryResponseTests
{
    [Fact]
    public void CategoryResponse_WithValidData_SetsPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var updatedBy = Guid.NewGuid();
        DateTime createdAt = DateTime.UtcNow;
        DateTime updatedAt = DateTime.UtcNow.AddHours(1);

        // Act
        var response = new CategoryResponse
        {
            Id = id,
            Active = true,
            CreatedBy = createdBy,
            CreatedAt = createdAt,
            UpdatedBy = updatedBy,
            UpdatedAt = updatedAt,
            Title = "Investimentos",
            Description = "Categoria de investimentos financeiros",
            UserId = userId
        };

        // Assert
        Assert.Equal(id, response.Id);
        Assert.True(response.Active);
        Assert.Equal(createdBy, response.CreatedBy);
        Assert.Equal(createdAt, response.CreatedAt);
        Assert.Equal(updatedBy, response.UpdatedBy);
        Assert.Equal(updatedAt, response.UpdatedAt);
        Assert.Equal("Investimentos", response.Title);
        Assert.Equal("Categoria de investimentos financeiros", response.Description);
        Assert.Equal(userId, response.UserId);
    }

    [Fact]
    public void CategoryResponse_WithExplicitNullAuditFields_AllowsNulls()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        DateTime createdAt = DateTime.UtcNow;

        // Act
        var response = new CategoryResponse
        {
            Id = id,
            Active = false,
            CreatedBy = createdBy,
            CreatedAt = createdAt,
            UpdatedBy = null,
            UpdatedAt = null,
            Title = "Alimentação",
            Description = "Despesas com supermercado e alimentação",
            UserId = userId
        };

        // Assert
        Assert.Equal(id, response.Id);
        Assert.False(response.Active);
        Assert.Equal(createdBy, response.CreatedBy);
        Assert.Equal(createdAt, response.CreatedAt);
        Assert.Null(response.UpdatedBy);
        Assert.Null(response.UpdatedAt);
        Assert.Equal("Alimentação", response.Title);
        Assert.Equal("Despesas com supermercado e alimentação", response.Description);
    }
}
