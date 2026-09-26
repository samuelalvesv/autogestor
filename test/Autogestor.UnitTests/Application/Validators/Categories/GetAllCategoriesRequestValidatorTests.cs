using Autogestor.Application.Validators.Categories;
using Autogestor.Contract;
using Autogestor.Contract.Requests.Categories;
using FluentValidation.Results;

namespace Autogestor.UnitTests.Application.Validators.Categories;

public sealed class GetAllCategoriesRequestValidatorTests
{
    private readonly GetAllCategoriesRequestValidator _validator = new();

    [Theory]
    [InlineData(10)]
    [InlineData(25)]
    [InlineData(50)]
    public async Task ValidateAsync_WithValidPageSize_ReturnsValidResult(int pageSize)
    {
        // Arrange
        var request = new GetAllCategoriesRequest
        {
            PageSize = pageSize,
            Cursor = null
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.True(condition: result.IsValid);
        Assert.Empty(collection: result.Errors);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(51)]
    [InlineData(100)]
    public async Task ValidateAsync_WithInvalidPageSize_ReturnsError(int pageSize)
    {
        // Arrange
        var request = new GetAllCategoriesRequest
        {
            PageSize = pageSize,
            Cursor = null
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(instance: request, cancellation: TestContext.Current.CancellationToken);

        // Assert
        Assert.False(condition: result.IsValid);
        Assert.Contains(collection: result.Errors, filter: static e => e.ErrorMessage == $"O tamanho da página deve estar entre {ContractDefaults.MinPageSize} e {ContractDefaults.MaxPageSize}.");
    }
}
