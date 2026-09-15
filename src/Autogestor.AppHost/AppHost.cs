IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args: args);

IResourceBuilder<ProjectResource> api = builder.AddProject<Projects.Autogestor_Api>(name: "api");

builder.AddProject<Projects.Autogestor_Web>(name: "web")
    .WithReference(source: api);

builder.Build().Run();
