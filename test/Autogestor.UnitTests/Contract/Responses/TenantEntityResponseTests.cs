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
        Assert.Equal(expected: id, actual: response.Id);
        Assert.True(condition: response.Active, userMessage: "O DTO da entidade de tenant deve reportar estado ativo.");
        Assert.Equal(expected: createdBy, actual: response.CreatedBy);
        Assert.Equal(expected: createdAt, actual: response.CreatedAt);
        Assert.Equal(expected: updatedBy, actual: response.UpdatedBy);
        Assert.Equal(expected: updatedAt, actual: response.UpdatedAt);
        Assert.Equal(expected: tenantId, actual: response.TenantId);
        Assert.IsAssignableFrom<AuditableEntityResponse>(@object: response);
    }
}
