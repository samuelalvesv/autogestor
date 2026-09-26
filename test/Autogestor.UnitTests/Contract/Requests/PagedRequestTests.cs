using Autogestor.Contract;
using Autogestor.Contract.Requests;

namespace Autogestor.UnitTests.Contract.Requests;

public sealed class PagedRequestTests
{
    private sealed record TestPagedRequest : PagedRequest;

    [Fact]
    public void PagedRequest_WithValues_SetsPropertiesCorrectly()
    {
        // Arrange
        var cursor = Guid.NewGuid();

        // Act
        var request = new TestPagedRequest
        {
            Cursor = cursor,
            PageSize = ContractDefaults.DefaultPageSize
        };

        // Assert
        Assert.Equal(expected: cursor, actual: request.Cursor);
        Assert.Equal(expected: ContractDefaults.DefaultPageSize, actual: request.PageSize);
    }

    [Fact]
    public void PagedRequest_WithNullCursor_SetsCursorToNull()
    {
        // Act
        var request = new TestPagedRequest
        {
            Cursor = null,
            PageSize = 20
        };

        // Assert
        Assert.Null(@object: request.Cursor);
        Assert.Equal(expected: 20, actual: request.PageSize);
    }

    [Fact]
    public void PagedRequest_RecordEquality_ReturnsTrueForEqualValues()
    {
        // Arrange
        var cursor = Guid.NewGuid();
        var request1 = new TestPagedRequest { Cursor = cursor, PageSize = 25 };
        var request2 = new TestPagedRequest { Cursor = cursor, PageSize = 25 };

        // Act & Assert
        Assert.Equal(expected: request1, actual: request2);
    }
}
