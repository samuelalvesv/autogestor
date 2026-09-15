using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Autogestor.Api.Middlewares;

public sealed class GrpcExceptionInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request: request, context: context);
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(
                status: new Status(
                    statusCode: StatusCode.InvalidArgument,
                    detail: ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            throw new RpcException(
                status: new Status(
                    statusCode: StatusCode.FailedPrecondition,
                    detail: ex.Message));
        }
    }
}
