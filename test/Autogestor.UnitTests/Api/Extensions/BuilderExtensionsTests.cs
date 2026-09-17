using Autogestor.Api.Extensions;
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
}
