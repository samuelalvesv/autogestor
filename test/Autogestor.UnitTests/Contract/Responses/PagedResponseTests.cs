using Autogestor.Contract.Responses;

namespace Autogestor.UnitTests.Contract.Responses;

public class PagedResponseTests
{
    [Theory]
    [InlineData(100, 25, 4)]
    [InlineData(101, 25, 5)]
    [InlineData(0, 25, 0)]
    [InlineData(1, 10, 1)]
    [InlineData(50, 0, 0)] // Guard against division by zero
    [InlineData(50, -5, 0)]
    public void TotalPage_ShouldCalculateCorrectly(int totalCount, int pageSize, int expectedTotalPages)
    {
        // Act
        var pagedResponse = new PagedResponse<string>
        {
            Data = ["item1", "item2"],
            Message = null,
            TotalCount = totalCount,
            CurrentPage = 1,
            PageSize = pageSize
        };

        // Assert
        Assert.Equal(expectedTotalPages, pagedResponse.TotalPage);
        Assert.Equal(totalCount, pagedResponse.TotalCount);
        Assert.Equal(1, pagedResponse.CurrentPage);
        Assert.Equal(pageSize, pagedResponse.PageSize);
        Assert.NotNull(pagedResponse.Data);
        Assert.Equal(2, pagedResponse.Data.Count);
    }

    [Fact]
    public void PagedResponse_ExplicitInitialization_ShouldHaveCorrectValues()
    {
        // Act
        var pagedResponse = new PagedResponse<object?>
        {
            Data = null,
            Message = null,
            TotalCount = 0,
            CurrentPage = 1,
            PageSize = 25
        };

        // Assert
        Assert.Null(pagedResponse.Data);
        Assert.Null(pagedResponse.Message);
        Assert.Equal(0, pagedResponse.TotalCount);
        Assert.Equal(1, pagedResponse.CurrentPage);
        Assert.Equal(25, pagedResponse.PageSize);
        Assert.Equal(0, pagedResponse.TotalPage);
    }

    [Fact]
    public void PagedResponse_WithMessage_ShouldSetMessageCorrectly()
    {
        // Act
        var pagedResponse = new PagedResponse<string>
        {
            Data = ["item"],
            Message = "List loaded",
            TotalCount = 1,
            CurrentPage = 1,
            PageSize = 25
        };

        // Assert
        Assert.Equal("List loaded", pagedResponse.Message);
        Assert.NotNull(pagedResponse.Data);
        Assert.Single(pagedResponse.Data);
    }
}
