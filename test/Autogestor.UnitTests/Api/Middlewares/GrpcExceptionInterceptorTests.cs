using Autogestor.Api.Middlewares;
using Grpc.Core;

namespace Autogestor.UnitTests.Api.Middlewares;

public sealed class GrpcExceptionInterceptorTests
{
    private sealed class TestServerCallContext : ServerCallContext
    {
        protected override string MethodCore => "TestMethod";
        protected override string HostCore => "localhost";
        protected override string PeerCore => "127.0.0.1";
        protected override DateTime DeadlineCore => DateTime.UtcNow.AddMinutes(1);
        protected override Metadata RequestHeadersCore => [];
        protected override CancellationToken CancellationTokenCore => CancellationToken.None;
        protected override Metadata ResponseTrailersCore => [];
        protected override Status StatusCore { get; set; }
        protected override WriteOptions? WriteOptionsCore { get; set; }
        protected override AuthContext AuthContextCore => new(peerIdentityPropertyName: null, properties: []);
        protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions? options) => throw new NotImplementedException();
        protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders) => Task.CompletedTask;
    }

    [Fact]
    public async Task UnaryServerHandler_WhenContinuationSucceeds_ReturnsResponse()
    {
        // Arrange
        var interceptor = new GrpcExceptionInterceptor();
        var context = new TestServerCallContext();
        const string expectedResponse = "Sucesso";

        // Act
        string actualResponse = await interceptor.UnaryServerHandler(
            request: "request_payload",
            context: context,
            continuation: (req, ctx) => Task.FromResult(result: expectedResponse));

        // Assert
        Assert.Equal(expected: expectedResponse, actual: actualResponse);
    }

    [Fact]
    public async Task UnaryServerHandler_WhenContinuationThrowsArgumentException_ThrowsRpcExceptionWithInvalidArgument()
    {
        // Arrange
        var interceptor = new GrpcExceptionInterceptor();
        var context = new TestServerCallContext();
        const string errorMessage = "Categoria não encontrada para o tenant atual.";

        // Act & Assert
        RpcException rpcException = await Assert.ThrowsAsync<RpcException>(
            testCode: () => interceptor.UnaryServerHandler<string, string>(
                request: "request_payload",
                context: context,
                continuation: (req, ctx) => throw new ArgumentException(
                    message: errorMessage,
                    paramName: "CategoryId")));

        Assert.Equal(expected: StatusCode.InvalidArgument, actual: rpcException.Status.StatusCode);
        Assert.Contains(expectedSubstring: errorMessage, actualString: rpcException.Status.Detail, comparisonType: StringComparison.Ordinal);
    }

    [Fact]
    public async Task UnaryServerHandler_WhenContinuationThrowsInvalidOperationException_ThrowsRpcExceptionWithFailedPrecondition()
    {
        // Arrange
        var interceptor = new GrpcExceptionInterceptor();
        var context = new TestServerCallContext();
        const string errorMessage = "Operação não permitida no estado atual.";

        // Act & Assert
        RpcException rpcException = await Assert.ThrowsAsync<RpcException>(
            testCode: () => interceptor.UnaryServerHandler<string, string>(
                request: "request_payload",
                context: context,
                continuation: (req, ctx) => throw new InvalidOperationException(message: errorMessage)));

        Assert.Equal(expected: StatusCode.FailedPrecondition, actual: rpcException.Status.StatusCode);
        Assert.Equal(expected: errorMessage, actual: rpcException.Status.Detail);
    }

    [Fact]
    public async Task UnaryServerHandler_WhenContinuationThrowsUnhandledException_RethrowsOriginalException()
    {
        // Arrange
        var interceptor = new GrpcExceptionInterceptor();
        var context = new TestServerCallContext();
        const string errorMessage = "Erro inesperado de infraestrutura.";

        // Act & Assert
        InvalidTimeZoneException exception = await Assert.ThrowsAsync<InvalidTimeZoneException>(
            testCode: () => interceptor.UnaryServerHandler<string, string>(
                request: "request_payload",
                context: context,
                continuation: (req, ctx) => throw new InvalidTimeZoneException(message: errorMessage)));

        Assert.Equal(expected: errorMessage, actual: exception.Message);
    }

    [Fact]
    public async Task UnaryServerHandler_WhenContinuationThrowsRpcException_RethrowsRpcExceptionUnchanged()
    {
        // Arrange
        var interceptor = new GrpcExceptionInterceptor();
        var context = new TestServerCallContext();
        var originalRpcException = new RpcException(
            status: new Status(
                statusCode: StatusCode.NotFound,
                detail: "Recurso não encontrado."));

        // Act & Assert
        RpcException rpcException = await Assert.ThrowsAsync<RpcException>(
            testCode: () => interceptor.UnaryServerHandler<string, string>(
                request: "request_payload",
                context: context,
                continuation: (req, ctx) => throw originalRpcException));

        Assert.Equal(expected: StatusCode.NotFound, actual: rpcException.Status.StatusCode);
        Assert.Equal(expected: "Recurso não encontrado.", actual: rpcException.Status.Detail);
    }
}
