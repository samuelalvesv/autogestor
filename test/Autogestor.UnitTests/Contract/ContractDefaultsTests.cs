using Autogestor.Contract;

namespace Autogestor.UnitTests.Contract;

public sealed class ContractDefaultsTests
{
    [Fact]
    public void Defaults_ShouldHaveExpectedValues()
    {
        Assert.Equal(expected: 25, actual: ContractDefaults.DefaultPageSize);
        Assert.Equal(expected: 10, actual: ContractDefaults.MinPageSize);
        Assert.Equal(expected: 50, actual: ContractDefaults.MaxPageSize);
    }
}
