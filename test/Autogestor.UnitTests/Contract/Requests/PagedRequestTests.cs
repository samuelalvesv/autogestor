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
            UserId = Guid.Empty,
            PageNumber = ContractDefaults.DefaultPageNumber,
            PageSize = ContractDefaults.DefaultPageSize
        };

        // Assert
        Assert.Equal(ContractDefaults.DefaultPageNumber, request.PageNumber);
        Assert.Equal(ContractDefaults.DefaultPageSize, request.PageSize);
        Assert.Equal(Guid.Empty, request.UserId);
    }

    [Fact]
    public void PagedRequest_ShouldAllowCustomValuesOnInit()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var request = new TestPagedRequest
        {
            PageNumber = 3,
            PageSize = 30,
            UserId = userId
        };

        // Assert
        Assert.Equal(3, request.PageNumber);
        Assert.Equal(30, request.PageSize);
        Assert.Equal(userId, request.UserId);
    }
}
