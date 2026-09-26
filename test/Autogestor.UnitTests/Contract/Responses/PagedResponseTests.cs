using Autogestor.Contract.Responses;

namespace Autogestor.UnitTests.Contract.Responses;

public sealed class PagedResponseTests
{
    [Fact]
    public void PagedResponse_WithHasNextPage_ShouldHaveNextCursor()
    {
        // Arrange & Act
        var nextCursor = Guid.NewGuid();
        var pagedResponse = new PagedResponse<string>
        {
            Data = ["item1", "item2"],
            HasNextPage = true,
            NextCursor = nextCursor
        };

        // Assert
        Assert.True(condition: pagedResponse.HasNextPage, userMessage: "Deve indicar que há próxima página.");
        Assert.Equal(expected: nextCursor, actual: pagedResponse.NextCursor);
        Assert.Equal(expected: 2, actual: pagedResponse.Data.Count);
    }

    [Fact]
    public void PagedResponse_WithNoNextPage_ShouldHaveNullCursor()
    {
        // Arrange & Act
        var pagedResponse = new PagedResponse<string>
        {
            Data = ["item1"],
            HasNextPage = false,
            NextCursor = null
        };

        // Assert
        Assert.False(condition: pagedResponse.HasNextPage, userMessage: "Não deve indicar próxima página.");
        Assert.Null(@object: pagedResponse.NextCursor);
        Assert.Single(collection: pagedResponse.Data);
    }

    [Fact]
    public void PagedResponse_WithEmptyData_ShouldHaveCorrectValues()
    {
        // Arrange & Act
        var pagedResponse = new PagedResponse<string>
        {
            Data = [],
            HasNextPage = false,
            NextCursor = null
        };

        // Assert
        Assert.Empty(collection: pagedResponse.Data);
        Assert.False(condition: pagedResponse.HasNextPage, userMessage: "Lista vazia não deve ter próxima página.");
        Assert.Null(@object: pagedResponse.NextCursor);
    }
}
