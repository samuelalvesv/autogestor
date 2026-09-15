using Autogestor.Contract.Responses;

namespace Autogestor.UnitTests.Contract.Responses;

public sealed class DeleteResponseTests
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
        Assert.Equal(expected: id, actual: response.Id);
    }
}
