using Autogestor.Web.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args: args);

builder.ConfigureRootComponents()
    .ConfigureServices();

await builder.Build().RunAsync();
