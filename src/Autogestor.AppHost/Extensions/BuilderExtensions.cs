namespace Autogestor.AppHost.Extensions;

public static class BuilderExtensions
{
    public static IDistributedApplicationBuilder AddApplicationProjects(this IDistributedApplicationBuilder builder)
    {
        IResourceBuilder<ProjectResource> api = builder.AddProject<Projects.Autogestor_Api>(name: "api");

        builder.AddProject<Projects.Autogestor_Web>(name: "web")
            .WithReference(source: api);

        return builder;
    }
}
