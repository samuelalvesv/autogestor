using Autogestor.Contract.Responses.Categories;

namespace Autogestor.UnitTests.Contract.Responses.Categories;

public class CategoryResponseTests
{
    [Fact]
    public void CategoryResponse_WithValidData_SetsPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
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
            TenantId = tenantId,
            Title = "Investimentos",
            Description = "Categoria de investimentos financeiros"
        };

        // Assert
        Assert.Equal(expected: id, actual: response.Id);
        Assert.True(condition: response.Active, userMessage: "O DTO da categoria deve reportar estado ativo.");
        Assert.Equal(expected: createdBy, actual: response.CreatedBy);
        Assert.Equal(expected: createdAt, actual: response.CreatedAt);
        Assert.Equal(expected: updatedBy, actual: response.UpdatedBy);
        Assert.Equal(expected: updatedAt, actual: response.UpdatedAt);
        Assert.Equal(expected: tenantId, actual: response.TenantId);
        Assert.Equal(expected: "Investimentos", actual: response.Title);
        Assert.Equal(expected: "Categoria de investimentos financeiros", actual: response.Description);
    }

    [Fact]
    public void CategoryResponse_WithExplicitNullAuditFields_AllowsNulls()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
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
            TenantId = tenantId,
            Title = "Alimentação",
            Description = "Despesas com supermercado e alimentação"
        };

        // Assert
        Assert.Equal(expected: id, actual: response.Id);
        Assert.False(condition: response.Active, userMessage: "O DTO da categoria deve reportar estado inativo.");
        Assert.Equal(expected: createdBy, actual: response.CreatedBy);
        Assert.Equal(expected: createdAt, actual: response.CreatedAt);
        Assert.Null(@object: response.UpdatedBy);
        Assert.Null(@object: response.UpdatedAt);
        Assert.Equal(expected: tenantId, actual: response.TenantId);
        Assert.Equal(expected: "Alimentação", actual: response.Title);
        Assert.Equal(expected: "Despesas com supermercado e alimentação", actual: response.Description);
    }
}
