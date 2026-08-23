using Autogestor.Contract;

namespace Autogestor.UnitTests.Contract;

public class ContractDefaultsTests
{
    [Fact]
    public void Defaults_ShouldHaveExpectedValues()
    {
        Assert.Equal(1, ContractDefaults.DefaultPageNumber);
        Assert.Equal(25, ContractDefaults.DefaultPageSize);
        Assert.Equal(1, ContractDefaults.MinPageNumber);
        Assert.Equal(int.MaxValue, ContractDefaults.MaxPageNumber);
        Assert.Equal(10, ContractDefaults.MinPageSize);
        Assert.Equal(50, ContractDefaults.MaxPageSize);
    }
}
