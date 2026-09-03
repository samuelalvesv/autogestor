using Autogestor.Contract.Responses;

namespace Autogestor.UnitTests.Contract.Responses;

public class DeleteResponseTests
{
    [Fact]
    public void DeleteResponse_WithValidId_SetsPropertyCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = new DeleteResponse
        {
            Id = id
        };

        // Assert
        Assert.Equal(id, response.Id);
    }

    [Fact]
    public void DeleteResponse_Equality_TwoIdenticalInstances_AreEqual()
    {
        // Arrange
        var id = Guid.NewGuid();

        var response1 = new DeleteResponse { Id = id };
        var response2 = new DeleteResponse { Id = id };

        // Assert
        Assert.Equal(response1, response2);
        Assert.True(response1 == response2);
    }

    [Fact]
    public void DeleteResponse_Equality_DifferentIds_AreNotEqual()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        var response1 = new DeleteResponse { Id = id1 };
        var response2 = new DeleteResponse { Id = id2 };

        // Assert
        Assert.NotEqual(response1, response2);
        Assert.True(response1 != response2);
    }
}
