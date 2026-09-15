using Autogestor.Contract.Services;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using ProtoBuf.Grpc.Client;

namespace Autogestor.Web.Extensions;

public static class BuilderExtensions
{
    public static WebAssemblyHostBuilder ConfigureRootComponents(this WebAssemblyHostBuilder builder)
    {
        builder.RootComponents.Add<App>(selector: "#app");
        builder.RootComponents.Add<HeadOutlet>(selector: "head::after");

        return builder;
    }

    public static WebAssemblyHostBuilder ConfigureServices(this WebAssemblyHostBuilder builder)
    {
        var baseAddress = new Uri(uriString: builder.HostEnvironment.BaseAddress);

        builder.Services.AddMudServices();

        builder.Services.AddScoped(implementationFactory: _ => new HttpClient
        {
            BaseAddress = baseAddress
        });

        builder.Services.AddScoped(implementationFactory: _ =>
        {
            var handler = new GrpcWebHandler(mode: GrpcWebMode.GrpcWebText, innerHandler: new HttpClientHandler());
            return GrpcChannel.ForAddress(
                address: baseAddress,
                channelOptions: new GrpcChannelOptions
                {
                    HttpHandler = handler
                });
        });

        builder.Services.AddScoped(implementationFactory: sp =>
            sp.GetRequiredService<GrpcChannel>().CreateGrpcService<ITransactionService>());

        builder.Services.AddScoped(implementationFactory: sp =>
            sp.GetRequiredService<GrpcChannel>().CreateGrpcService<ICategoryService>());

        return builder;
    }
}
