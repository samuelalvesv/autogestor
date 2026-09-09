using Autogestor.Contract;
using Autogestor.Contract.Requests;

namespace Autogestor.UnitTests.Contract.Requests;

public class PagedRequestTests
{
    private sealed record TestPagedRequest : PagedRequest;

    [Fact]
    public void PagedRequest_ShouldRequireAllPropertiesOnInitialization()
    {
        // Act
        var request = new TestPagedRequest
        {
            PageNumber = ContractDefaults.DefaultPageNumber,
            PageSize = ContractDefaults.DefaultPageSize
        };

        // Assert
        Assert.Equal(expected: ContractDefaults.DefaultPageNumber, actual: request.PageNumber);
        Assert.Equal(expected: ContractDefaults.DefaultPageSize, actual: request.PageSize);
    }

    [Fact]
    public void PagedRequest_ShouldAllowCustomValuesOnInit()
    {
        // Act
        var request = new TestPagedRequest
        {
            PageNumber = 3,
            PageSize = 30
        };

        // Assert
        Assert.Equal(expected: 3, actual: request.PageNumber);
        Assert.Equal(expected: 30, actual: request.PageSize);
    }
}
