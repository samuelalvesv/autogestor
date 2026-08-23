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
        var pagedResponse = new PagedResponse<IReadOnlyList<string>>(
            data: ["item1", "item2"],
            code: 200,
            message: "Success",
            totalCount: totalCount,
            currentPage: 1,
            pageSize: pageSize
        );

        // Assert
        Assert.Equal(expectedTotalPages, pagedResponse.TotalPage);
        Assert.Equal(totalCount, pagedResponse.TotalCount);
        Assert.Equal(1, pagedResponse.CurrentPage);
        Assert.Equal(pageSize, pagedResponse.PageSize);
        Assert.True(pagedResponse.IsSuccess);
    }
}
