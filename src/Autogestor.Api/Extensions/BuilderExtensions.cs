using Autogestor.Api.Middlewares;
using Autogestor.Api.Services;
using Autogestor.Application.UseCases.Categories.Commands.CreateCategory;
using Autogestor.Application.UseCases.Categories.Commands.DeleteCategory;
using Autogestor.Application.UseCases.Categories.Commands.UpdateCategory;
using Autogestor.Application.UseCases.Categories.Queries.GetCategoryById;
using Autogestor.Application.UseCases.Transactions.Commands.CreateTransaction;
using Autogestor.Application.UseCases.Transactions.Commands.DeleteTransaction;
using Autogestor.Application.UseCases.Transactions.Commands.UpdateTransaction;
using Autogestor.Application.UseCases.Transactions.Queries.GetTransactionById;
using Autogestor.Contract.Services;
using Autogestor.Infrastructure;
using Autogestor.Infrastructure.Persistence;
using Autogestor.Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using ProtoBuf.Grpc.Server;

namespace Autogestor.Api.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder ConfigureKestrelProtocols(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(configureOptions: (_, options) =>
        {
            // gRPC-Web (HTTP/1.1) — Blazor WASM
            options.ListenAnyIP(port: 5132, configure: o => o.Protocols = HttpProtocols.Http1);

            // gRPC Native (HTTP/2 h2c) — Blazor Hybrid, Postman, grpcui, grpcurl
            options.ListenAnyIP(port: 5133, configure: o => o.Protocols = HttpProtocols.Http2);
        });

        return builder;
    }

    public static WebApplicationBuilder AddDatabasePersistence(this WebApplicationBuilder builder)
    {
        string connectionString = builder.Configuration.GetConnectionString(name: "DefaultConnection")
            ?? throw new ArgumentException(message: "String de conexão não encontrada");

        builder.Services.AddInfrastructure();

        builder.Services.AddDbContext<AppDbContext>(optionsAction: (serviceProvider, options) =>
        {
            options.UseNpgsql(
                connectionString: connectionString,
                npgsqlOptionsAction: b => b.MigrationsAssembly(assemblyName: "Autogestor.Infrastructure"))
                .UseSnakeCaseNamingConvention();
            options.AddInterceptors(interceptors:
            [
                serviceProvider.GetRequiredService<AuditableEntityInterceptor>(),
                serviceProvider.GetRequiredService<TenantEntityInterceptor>()
            ]);
        });

        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICreateCategoryUseCase, CreateCategoryUseCase>();
        builder.Services.AddScoped<IDeleteCategoryUseCase, DeleteCategoryUseCase>();
        builder.Services.AddScoped<IGetCategoryByIdUseCase, GetCategoryByIdUseCase>();
        builder.Services.AddScoped<IUpdateCategoryUseCase, UpdateCategoryUseCase>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<ICreateTransactionUseCase, CreateTransactionUseCase>();
        builder.Services.AddScoped<IDeleteTransactionUseCase, DeleteTransactionUseCase>();
        builder.Services.AddScoped<IGetTransactionByIdUseCase, GetTransactionByIdUseCase>();
        builder.Services.AddScoped<IUpdateTransactionUseCase, UpdateTransactionUseCase>();
        builder.Services.AddScoped<ITransactionService, TransactionService>();

        return builder;
    }

    public static WebApplicationBuilder AddGrpcConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.AddCodeFirstGrpc(configureOptions: options => options.Interceptors.Add<GrpcExceptionInterceptor>());
        builder.Services.AddCodeFirstGrpcReflection();

        return builder;
    }

    public static WebApplicationBuilder AddCorsPolicy(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(setupAction: options => options.AddDefaultPolicy(configurePolicy: policy => policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .WithExposedHeaders(exposedHeaders: ["Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding"])));

        return builder;
    }
}
