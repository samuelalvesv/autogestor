using Autogestor.Contract;
using Autogestor.Contract.Requests.Categories;

namespace Autogestor.UnitTests.Contract.Requests.Categories;

public class GetAllCategoriesRequestTests
{
    [Fact]
    public void GetAllCategoriesRequest_InheritsPagedRequest_SetsPaginationDefaults()
    {
        // Act
        var request = new GetAllCategoriesRequest
        {
            PageNumber = ContractDefaults.DefaultPageNumber,
            PageSize = ContractDefaults.DefaultPageSize
        };

        // Assert
        Assert.Equal(expected: ContractDefaults.DefaultPageNumber, actual: request.PageNumber);
        Assert.Equal(expected: ContractDefaults.DefaultPageSize, actual: request.PageSize);
    }

    [Fact]
    public void GetAllCategoriesRequest_AllowsCustomPaginationValues()
    {
        // Act
        var request = new GetAllCategoriesRequest
        {
            PageNumber = 2,
            PageSize = 50
        };

        // Assert
        Assert.Equal(expected: 2, actual: request.PageNumber);
        Assert.Equal(expected: 50, actual: request.PageSize);
    }
}
