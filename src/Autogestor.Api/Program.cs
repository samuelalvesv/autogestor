using Autogestor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new ArgumentException($"String de conexão não encontrada");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, b =>
        b.MigrationsAssembly("Autogestor.Infrastructure")));

WebApplication app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
