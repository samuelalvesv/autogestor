using Autogestor.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

builder.AddServiceDefaults();
builder.ConfigureKestrelProtocols();
builder.AddDatabasePersistence();
builder.AddApplicationServices();
builder.AddGrpcConfiguration();
builder.AddCorsPolicy();

WebApplication app = builder.Build();

app.UseApiPipeline();
app.MapGrpcEndpoints();
app.MapDefaultEndpoints();

app.Run();
