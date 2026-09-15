using Autogestor.Contract;

namespace Autogestor.UnitTests.Contract;

public sealed class ContractDefaultsTests
{
    [Fact]
    public void Defaults_ShouldHaveExpectedValues()
    {
        Assert.Equal(expected: 1, actual: ContractDefaults.DefaultPageNumber);
        Assert.Equal(expected: 25, actual: ContractDefaults.DefaultPageSize);
        Assert.Equal(expected: 1, actual: ContractDefaults.MinPageNumber);
        Assert.Equal(expected: int.MaxValue, actual: ContractDefaults.MaxPageNumber);
        Assert.Equal(expected: 10, actual: ContractDefaults.MinPageSize);
        Assert.Equal(expected: 50, actual: ContractDefaults.MaxPageSize);
    }
}
