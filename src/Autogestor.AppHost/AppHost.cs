using Autogestor.AppHost.Extensions;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args: args);

builder.ConfigureAppTopology();

builder.Build().Run();
