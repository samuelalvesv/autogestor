using Autogestor.Contract.Responses;

namespace Autogestor.UnitTests.Contract.Responses;

public class ResponseTests
{
    [Fact]
    public void Response_ShouldSetDataAndMessageExplicitly()
    {
        // Act
        var response = new Response<string>
        {
            Data = "test-data",
            Message = "Operation successful"
        };

        // Assert
        Assert.Equal("test-data", response.Data);
        Assert.Equal("Operation successful", response.Message);
    }

    [Fact]
    public void Response_ShouldAllowExplicitNulls()
    {
        // Act
        var response = new Response<string?>
        {
            Data = null,
            Message = null
        };

        // Assert
        Assert.Null(response.Data);
        Assert.Null(response.Message);
    }

    [Fact]
    public void Response_WithIntegerData_ShouldSetCorrectly()
    {
        // Act
        var response = new Response<int>
        {
            Data = 42,
            Message = "Created"
        };

        // Assert
        Assert.Equal(42, response.Data);
        Assert.Equal("Created", response.Message);
    }
}
