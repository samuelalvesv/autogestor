using Autogestor.Contract.Requests;

namespace Autogestor.UnitTests.Contract.Requests;

public class RequestTests
{
    private sealed record TestRequest : Request;

    [Fact]
    public void Request_ShouldRequireUserIdOnInitialization()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var request = new TestRequest
        {
            UserId = userId
        };

        // Assert
        Assert.Equal(userId, request.UserId);
    }
}
