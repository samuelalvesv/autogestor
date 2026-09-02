using Autogestor.Contract;
using Autogestor.Contract.Requests.Categories;

namespace Autogestor.UnitTests.Contract.Requests.Categories;

public class GetAllCategoriesRequestTests
{
    [Fact]
    public void GetAllCategoriesRequest_InheritsPagedRequest_SetsPaginationDefaults()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var request = new GetAllCategoriesRequest
        {
            UserId = userId,
            PageNumber = ContractDefaults.DefaultPageNumber,
            PageSize = ContractDefaults.DefaultPageSize
        };

        // Assert
        Assert.Equal(userId, request.UserId);
        Assert.Equal(ContractDefaults.DefaultPageNumber, request.PageNumber);
        Assert.Equal(ContractDefaults.DefaultPageSize, request.PageSize);
    }

    [Fact]
    public void GetAllCategoriesRequest_AllowsCustomPaginationValues()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var request = new GetAllCategoriesRequest
        {
            UserId = userId,
            PageNumber = 2,
            PageSize = 50
        };

        // Assert
        Assert.Equal(userId, request.UserId);
        Assert.Equal(2, request.PageNumber);
        Assert.Equal(50, request.PageSize);
    }
}
