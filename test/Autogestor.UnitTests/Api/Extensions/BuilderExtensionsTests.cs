using Autogestor.Api.Extensions;
using Autogestor.Application.Validators.Categories;
using Autogestor.Application.Validators.Transactions;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Requests.Transactions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Autogestor.UnitTests.Api.Extensions;

public sealed class BuilderExtensionsTests
{
    [Fact]
    public void ConfigureKestrelProtocols_ShouldConfigureKestrelWithoutThrowing()
    {
        // Arrange
        WebApplicationBuilder builder = WebApplication.CreateBuilder();

        // Act
        builder.ConfigureKestrelProtocols();
        using WebApplication app = builder.Build();
        KestrelServerOptions kestrelOptions = app.Services.GetRequiredService<IOptions<KestrelServerOptions>>().Value;

        // Assert
        Assert.NotNull(@object: kestrelOptions);
    }

    [Fact]
    public void AddApplicationServices_RegistersAllValidators_ResolvableFromServiceProvider()
    {
        // Arrange
        WebApplicationBuilder builder = WebApplication.CreateBuilder();

        // Act
        builder.AddApplicationServices();
        using ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();

        // Assert - Category Validators
        IValidator<CreateCategoryRequest> createCategoryValidator = serviceProvider.GetRequiredService<IValidator<CreateCategoryRequest>>();
        Assert.IsType<CreateCategoryRequestValidator>(@object: createCategoryValidator);

        IValidator<UpdateCategoryRequest> updateCategoryValidator = serviceProvider.GetRequiredService<IValidator<UpdateCategoryRequest>>();
        Assert.IsType<UpdateCategoryRequestValidator>(@object: updateCategoryValidator);

        IValidator<DeleteCategoryRequest> deleteCategoryValidator = serviceProvider.GetRequiredService<IValidator<DeleteCategoryRequest>>();
        Assert.IsType<DeleteCategoryRequestValidator>(@object: deleteCategoryValidator);

        IValidator<GetCategoryByIdRequest> getCategoryByIdValidator = serviceProvider.GetRequiredService<IValidator<GetCategoryByIdRequest>>();
        Assert.IsType<GetCategoryByIdRequestValidator>(@object: getCategoryByIdValidator);

        IValidator<GetAllCategoriesRequest> getAllCategoriesValidator = serviceProvider.GetRequiredService<IValidator<GetAllCategoriesRequest>>();
        Assert.IsType<GetAllCategoriesRequestValidator>(@object: getAllCategoriesValidator);

        // Assert - Transaction Validators
        IValidator<CreateTransactionRequest> createTransactionValidator = serviceProvider.GetRequiredService<IValidator<CreateTransactionRequest>>();
        Assert.IsType<CreateTransactionRequestValidator>(@object: createTransactionValidator);

        IValidator<UpdateTransactionRequest> updateTransactionValidator = serviceProvider.GetRequiredService<IValidator<UpdateTransactionRequest>>();
        Assert.IsType<UpdateTransactionRequestValidator>(@object: updateTransactionValidator);

        IValidator<DeleteTransactionRequest> deleteTransactionValidator = serviceProvider.GetRequiredService<IValidator<DeleteTransactionRequest>>();
        Assert.IsType<DeleteTransactionRequestValidator>(@object: deleteTransactionValidator);

        IValidator<GetTransactionByIdRequest> getTransactionByIdValidator = serviceProvider.GetRequiredService<IValidator<GetTransactionByIdRequest>>();
        Assert.IsType<GetTransactionByIdRequestValidator>(@object: getTransactionByIdValidator);

        IValidator<GetAllTransactionsRequest> getAllTransactionsValidator = serviceProvider.GetRequiredService<IValidator<GetAllTransactionsRequest>>();
        Assert.IsType<GetAllTransactionsRequestValidator>(@object: getAllTransactionsValidator);
    }
}
