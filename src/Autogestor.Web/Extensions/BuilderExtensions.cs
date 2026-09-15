using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

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
        builder.Services.AddScoped(implementationFactory: _ => new HttpClient
        {
            BaseAddress = new Uri(uriString: builder.HostEnvironment.BaseAddress)
        });

        return builder;
    }
}
