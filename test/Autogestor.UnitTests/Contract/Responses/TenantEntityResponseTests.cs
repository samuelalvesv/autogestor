using Autogestor.Contract.Responses;

namespace Autogestor.UnitTests.Contract.Responses;

public class TenantEntityResponseTests
{
    private sealed record TestTenantEntityResponse : TenantEntityResponse;

    [Fact]
    public void TenantEntityResponse_WithValidData_SetsPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var updatedBy = Guid.NewGuid();
        DateTime createdAt = DateTime.UtcNow;
        DateTime updatedAt = DateTime.UtcNow.AddMinutes(5);

        // Act
        var response = new TestTenantEntityResponse
        {
            Id = id,
            Active = true,
            CreatedBy = createdBy,
            CreatedAt = createdAt,
            UpdatedBy = updatedBy,
            UpdatedAt = updatedAt,
            TenantId = tenantId
        };

        // Assert
        Assert.Equal(id, response.Id);
        Assert.True(response.Active);
        Assert.Equal(createdBy, response.CreatedBy);
        Assert.Equal(createdAt, response.CreatedAt);
        Assert.Equal(updatedBy, response.UpdatedBy);
        Assert.Equal(updatedAt, response.UpdatedAt);
        Assert.Equal(tenantId, response.TenantId);
        Assert.IsAssignableFrom<AuditableEntityResponse>(response);
    }
}
