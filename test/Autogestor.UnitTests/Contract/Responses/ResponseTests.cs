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
        Assert.Equal(expected: "test-data", actual: response.Data);
        Assert.Equal(expected: "Operation successful", actual: response.Message);
    }

    [Fact]
    public void Response_WithNullData_ShouldAllowNullData()
    {
        // Act
        var response = new Response<string?>
        {
            Data = null,
            Message = "Sem dados"
        };

        // Assert
        Assert.Null(@object: response.Data);
        Assert.Equal(expected: "Sem dados", actual: response.Message);
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
        Assert.Equal(expected: 42, actual: response.Data);
        Assert.Equal(expected: "Created", actual: response.Message);
    }
}
