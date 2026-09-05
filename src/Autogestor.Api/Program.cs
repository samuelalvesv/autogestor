using Autogestor.Api.Services;
using Autogestor.Application.UseCases.Categories.Commands.CreateCategory;
using Autogestor.Contract.Services;
using Autogestor.Infrastructure;
using Autogestor.Infrastructure.Persistence;
using Autogestor.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

string connectionString = builder.Configuration.GetConnectionString(name: "DefaultConnection")
    ?? throw new ArgumentException(message: "String de conexão não encontrada");

builder.Services.AddInfrastructure();

builder.Services.AddDbContext<AppDbContext>(optionsAction: (serviceProvider, options) =>
{
    options.UseNpgsql(
        connectionString: connectionString,
        npgsqlOptionsAction: b => b.MigrationsAssembly(assemblyName: "Autogestor.Infrastructure"));
    options.AddInterceptors(interceptors: serviceProvider.GetRequiredService<AuditableEntityInterceptor>());
});

builder.Services.AddScoped<ICreateCategoryUseCase, CreateCategoryUseCase>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

WebApplication app = builder.Build();

app.Run();
