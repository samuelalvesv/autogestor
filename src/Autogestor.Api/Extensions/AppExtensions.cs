using Autogestor.Api.Services;
using ProtoBuf.Grpc.Server;

namespace Autogestor.Api.Extensions;

public static class AppExtensions
{
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        app.UseRouting();
        app.UseGrpcWeb();
        app.UseCors();

        return app;
    }

    public static WebApplication MapGrpcEndpoints(this WebApplication app)
    {
        app.MapGrpcService<CategoryService>().EnableGrpcWeb();
        app.MapGrpcService<TransactionService>().EnableGrpcWeb();
        app.MapCodeFirstGrpcReflectionService().EnableGrpcWeb();

        app.MapGet(pattern: "/", handler: () => "Comunicação com endpoints gRPC deve ser realizada através de um cliente gRPC.");

        return app;
    }
}
