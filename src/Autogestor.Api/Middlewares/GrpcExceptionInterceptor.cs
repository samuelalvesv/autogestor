using Autogestor.Domain.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Autogestor.Api.Middlewares;

public sealed partial class GrpcExceptionInterceptor(ILogger<GrpcExceptionInterceptor> logger) : Interceptor
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Erro inesperado ao processar requisição gRPC.")]
    private static partial void LogUnexpectedError(ILogger logger, Exception exception);

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request: request, context: context);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
        {
            throw new RpcException(status: new Status(statusCode: StatusCode.Cancelled, detail: "Operação cancelada pelo cliente."));
        }
        catch (DomainException ex)
        {
            StatusCode statusCode = ex.ErrorType switch
            {
                DomainErrorType.NotFound => StatusCode.NotFound,
                DomainErrorType.Validation => StatusCode.InvalidArgument,
                DomainErrorType.BusinessRule => StatusCode.FailedPrecondition,
                DomainErrorType.Conflict => StatusCode.AlreadyExists,
                _ => StatusCode.Internal
            };

            throw new RpcException(status: new Status(statusCode: statusCode, detail: ex.Message));
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(status: new Status(statusCode: StatusCode.InvalidArgument, detail: ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            throw new RpcException(status: new Status(statusCode: StatusCode.FailedPrecondition, detail: ex.Message));
        }
        catch (Exception ex)
        {
            LogUnexpectedError(logger: logger, exception: ex);
            throw new RpcException(status: new Status(statusCode: StatusCode.Internal, detail: "Ocorreu um erro interno no servidor."));
        }
    }
}
