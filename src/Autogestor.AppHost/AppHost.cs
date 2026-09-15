using Autogestor.AppHost.Extensions;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args: args);

builder.AddApplicationProjects();

builder.Build().Run();
