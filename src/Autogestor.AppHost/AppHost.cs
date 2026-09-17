IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args: args);

IResourceBuilder<ProjectResource> api = builder.AddProject<Projects.Autogestor_Api>(name: "api", launchProfileName: null)
    .WithEndpoint(endpointName: "grpc-web", callback: endpoint =>
    {
        endpoint.Port = 5132;
        endpoint.TargetPort = 5132;
        endpoint.UriScheme = "http";
        endpoint.IsProxied = false;
    })
    .WithEndpoint(endpointName: "grpc-native", callback: endpoint =>
    {
        endpoint.Port = 5133;
        endpoint.TargetPort = 5133;
        endpoint.UriScheme = "http";
        endpoint.Transport = "http2";
        endpoint.IsProxied = false;
    });

builder.AddProject<Projects.Autogestor_Web>(name: "web")
    .WithReference(source: api);

builder.Build().Run();
