using Autogestor.Contract.Responses;

namespace Autogestor.UnitTests.Contract.Responses;

public class ResponseTests
{
    [Theory]
    [InlineData(200, true)]
    [InlineData(201, true)]
    [InlineData(204, true)]
    [InlineData(299, true)]
    [InlineData(199, false)]
    [InlineData(300, false)]
    [InlineData(400, false)]
    [InlineData(404, false)]
    [InlineData(500, false)]
    public void Response_IsSuccess_ShouldEvaluateCorrectlyBasedOnStatusCode(int statusCode, bool expectedSuccess)
    {
        // Act
        var response = new Response<string>("test-data", statusCode, "test message");

        // Assert
        Assert.Equal(expectedSuccess, response.IsSuccess);
        Assert.Equal("test-data", response.Data);
        Assert.Equal("test message", response.Message);
    }

    [Fact]
    public void Response_DefaultValues_ShouldBeNullDataAndEmptyMessage()
    {
        // Act
        var response = new Response<object?>(null, 200, string.Empty);

        // Assert
        Assert.Null(response.Data);
        Assert.Equal(string.Empty, response.Message);
        Assert.True(response.IsSuccess);
    }
}
