using Autogestor.Web.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args: args);

builder.ConfigureRootComponents();
builder.ConfigureServices();

#pragma warning disable CA2007 // ConfigureAwait is irrelevant in Blazor WebAssembly top-level statements
await builder.Build().RunAsync();
#pragma warning restore CA2007
