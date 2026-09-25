using Autogestor.Api.Middlewares;
using Autogestor.Domain.Exceptions;
using Grpc.Core;
using Microsoft.Extensions.Logging.Abstractions;

namespace Autogestor.UnitTests.Api.Middlewares;

public sealed class GrpcExceptionInterceptorTests
{
    private sealed class TestServerCallContext(CancellationToken cancellationToken = default) : ServerCallContext
    {
        protected override string MethodCore => "TestMethod";
        protected override string HostCore => "localhost";
        protected override string PeerCore => "127.0.0.1";
        protected override DateTime DeadlineCore => DateTime.UtcNow.AddMinutes(1);
        protected override Metadata RequestHeadersCore => [];
        protected override CancellationToken CancellationTokenCore => cancellationToken;
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
        var interceptor = new GrpcExceptionInterceptor(logger: NullLogger<GrpcExceptionInterceptor>.Instance);
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
    public async Task UnaryServerHandler_WhenThrowsNotFoundException_ThrowsRpcExceptionWithNotFound()
    {
        // Arrange
        var interceptor = new GrpcExceptionInterceptor(logger: NullLogger<GrpcExceptionInterceptor>.Instance);
        var context = new TestServerCallContext();
        const string errorMessage = "Recurso não encontrado.";

        // Act & Assert
        RpcException rpcException = await Assert.ThrowsAsync<RpcException>(
            testCode: () => interceptor.UnaryServerHandler<string, string>(
                request: "request_payload",
                context: context,
                continuation: (req, ctx) => throw new NotFoundException(message: errorMessage)));

        Assert.Equal(expected: StatusCode.NotFound, actual: rpcException.Status.StatusCode);
        Assert.Equal(expected: errorMessage, actual: rpcException.Status.Detail);
    }

    [Fact]
    public async Task UnaryServerHandler_WhenThrowsDomainValidationException_ThrowsRpcExceptionWithInvalidArgument()
    {
        // Arrange
        var interceptor = new GrpcExceptionInterceptor(logger: NullLogger<GrpcExceptionInterceptor>.Instance);
        var context = new TestServerCallContext();
        const string errorMessage = "Dados de entrada inválidos.";

        // Act & Assert
        RpcException rpcException = await Assert.ThrowsAsync<RpcException>(
            testCode: () => interceptor.UnaryServerHandler<string, string>(
                request: "request_payload",
                context: context,
                continuation: (req, ctx) => throw new DomainValidationException(message: errorMessage)));

        Assert.Equal(expected: StatusCode.InvalidArgument, actual: rpcException.Status.StatusCode);
        Assert.Equal(expected: errorMessage, actual: rpcException.Status.Detail);
    }

    [Fact]
    public async Task UnaryServerHandler_WhenThrowsBusinessRuleException_ThrowsRpcExceptionWithFailedPrecondition()
    {
        // Arrange
        var interceptor = new GrpcExceptionInterceptor(logger: NullLogger<GrpcExceptionInterceptor>.Instance);
        var context = new TestServerCallContext();
        const string errorMessage = "Violação de regra de negócio.";

        // Act & Assert
        RpcException rpcException = await Assert.ThrowsAsync<RpcException>(
            testCode: () => interceptor.UnaryServerHandler<string, string>(
                request: "request_payload",
                context: context,
                continuation: (req, ctx) => throw new BusinessRuleException(message: errorMessage)));

        Assert.Equal(expected: StatusCode.FailedPrecondition, actual: rpcException.Status.StatusCode);
        Assert.Equal(expected: errorMessage, actual: rpcException.Status.Detail);
    }

    [Fact]
    public async Task UnaryServerHandler_WhenThrowsConflictException_ThrowsRpcExceptionWithAlreadyExists()
    {
        // Arrange
        var interceptor = new GrpcExceptionInterceptor(logger: NullLogger<GrpcExceptionInterceptor>.Instance);
        var context = new TestServerCallContext();
        const string errorMessage = "Recurso já existente.";

        // Act & Assert
        RpcException rpcException = await Assert.ThrowsAsync<RpcException>(
            testCode: () => interceptor.UnaryServerHandler<string, string>(
                request: "request_payload",
                context: context,
                continuation: (req, ctx) => throw new ConflictException(message: errorMessage)));

        Assert.Equal(expected: StatusCode.AlreadyExists, actual: rpcException.Status.StatusCode);
        Assert.Equal(expected: errorMessage, actual: rpcException.Status.Detail);
    }

    [Fact]
    public async Task UnaryServerHandler_WhenContinuationThrowsArgumentException_ThrowsRpcExceptionWithInvalidArgument()
    {
        // Arrange
        var interceptor = new GrpcExceptionInterceptor(logger: NullLogger<GrpcExceptionInterceptor>.Instance);
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
        var interceptor = new GrpcExceptionInterceptor(logger: NullLogger<GrpcExceptionInterceptor>.Instance);
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
    public async Task UnaryServerHandler_WhenContinuationThrowsUnhandledException_ThrowsRpcExceptionWithInternal()
    {
        // Arrange
        var interceptor = new GrpcExceptionInterceptor(logger: NullLogger<GrpcExceptionInterceptor>.Instance);
        var context = new TestServerCallContext();
        const string errorMessage = "Erro inesperado de infraestrutura.";

        // Act & Assert
        RpcException rpcException = await Assert.ThrowsAsync<RpcException>(
            testCode: () => interceptor.UnaryServerHandler<string, string>(
                request: "request_payload",
                context: context,
                continuation: (req, ctx) => throw new InvalidTimeZoneException(message: errorMessage)));

        Assert.Equal(expected: StatusCode.Internal, actual: rpcException.Status.StatusCode);
        Assert.Equal(expected: "Ocorreu um erro interno no servidor.", actual: rpcException.Status.Detail);
    }

    [Fact]
    public async Task UnaryServerHandler_WhenContinuationThrowsRpcException_RethrowsRpcExceptionUnchanged()
    {
        // Arrange
        var interceptor = new GrpcExceptionInterceptor(logger: NullLogger<GrpcExceptionInterceptor>.Instance);
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

    [Fact]
    public async Task UnaryServerHandler_WhenClientCancelsRequest_ThrowsRpcExceptionWithCancelled()
    {
        // Arrange
        var interceptor = new GrpcExceptionInterceptor(logger: NullLogger<GrpcExceptionInterceptor>.Instance);
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var context = new TestServerCallContext(cancellationToken: cts.Token);

        // Act & Assert
        RpcException rpcException = await Assert.ThrowsAsync<RpcException>(
            testCode: () => interceptor.UnaryServerHandler<string, string>(
                request: "request_payload",
                context: context,
                continuation: (req, ctx) => throw new OperationCanceledException(token: ctx.CancellationToken)));

        Assert.Equal(expected: StatusCode.Cancelled, actual: rpcException.Status.StatusCode);
        Assert.Equal(expected: "Operação cancelada pelo cliente.", actual: rpcException.Status.Detail);
    }
}
