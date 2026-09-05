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
            Message = "Sucesso",
            TotalCount = totalCount,
            PageNumber = 1,
            PageSize = pageSize
        };

        // Assert
        Assert.Equal(expected: expectedTotalPages, actual: pagedResponse.TotalPage);
        Assert.Equal(expected: totalCount, actual: pagedResponse.TotalCount);
        Assert.Equal(expected: 1, actual: pagedResponse.PageNumber);
        Assert.Equal(expected: pageSize, actual: pagedResponse.PageSize);
        Assert.NotNull(@object: pagedResponse.Data);
        Assert.Equal(expected: 2, actual: pagedResponse.Data.Count);
    }

    [Fact]
    public void PagedResponse_ExplicitInitialization_ShouldHaveCorrectValues()
    {
        // Act
        var pagedResponse = new PagedResponse<object?>
        {
            Data = null,
            Message = "Sem dados",
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 25
        };

        // Assert
        Assert.Null(@object: pagedResponse.Data);
        Assert.Equal(expected: "Sem dados", actual: pagedResponse.Message);
        Assert.Equal(expected: 0, actual: pagedResponse.TotalCount);
        Assert.Equal(expected: 1, actual: pagedResponse.PageNumber);
        Assert.Equal(expected: 25, actual: pagedResponse.PageSize);
        Assert.Equal(expected: 0, actual: pagedResponse.TotalPage);
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
            PageNumber = 1,
            PageSize = 25
        };

        // Assert
        Assert.Equal(expected: "List loaded", actual: pagedResponse.Message);
        Assert.NotNull(@object: pagedResponse.Data);
        Assert.Single(collection: pagedResponse.Data);
    }
}
