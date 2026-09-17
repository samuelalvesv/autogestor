using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Autogestor.AppHost.Extensions;

namespace Autogestor.UnitTests.AppHost;

public sealed class AppHostTopologyTests
{
    [Fact]
    public void ConfigureAppTopology_ShouldRegisterApiAndWebResourcesWithCorrectEndpoints()
    {
        // Arrange
        IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder();

        // Act
        builder.ConfigureAppTopology();

        // Assert
        IResource? apiResource = builder.Resources.FirstOrDefault(predicate: r => r.Name == "api");
        Assert.NotNull(@object: apiResource);

        var apiWithEndpoints = apiResource as IResourceWithEndpoints;
        Assert.NotNull(@object: apiWithEndpoints);

        EndpointAnnotation? grpcWebEndpoint = apiWithEndpoints.Annotations
            .OfType<EndpointAnnotation>()
            .FirstOrDefault(predicate: e => e.Name == "grpc-web");
        Assert.NotNull(@object: grpcWebEndpoint);
        Assert.Equal(expected: 5132, actual: grpcWebEndpoint.Port);
        Assert.Equal(expected: 5132, actual: grpcWebEndpoint.TargetPort);
        Assert.Equal(expected: "http", actual: grpcWebEndpoint.UriScheme);
        Assert.False(condition: grpcWebEndpoint.IsProxied, userMessage: "grpc-web não deve ser roteado pelo proxy.");

        EndpointAnnotation? grpcNativeEndpoint = apiWithEndpoints.Annotations
            .OfType<EndpointAnnotation>()
            .FirstOrDefault(predicate: e => e.Name == "grpc-native");
        Assert.NotNull(@object: grpcNativeEndpoint);
        Assert.Equal(expected: 5133, actual: grpcNativeEndpoint.Port);
        Assert.Equal(expected: 5133, actual: grpcNativeEndpoint.TargetPort);
        Assert.Equal(expected: "http", actual: grpcNativeEndpoint.UriScheme);
        Assert.Equal(expected: "http2", actual: grpcNativeEndpoint.Transport);
        Assert.False(condition: grpcNativeEndpoint.IsProxied, userMessage: "grpc-native não deve ser roteado pelo proxy.");

        IResource? webResource = builder.Resources.FirstOrDefault(predicate: r => r.Name == "web");
        Assert.NotNull(@object: webResource);
    }
}
