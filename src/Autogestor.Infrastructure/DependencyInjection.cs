using Autogestor.Domain.Interfaces;
using Autogestor.Infrastructure.Persistence;
using Autogestor.Infrastructure.Persistence.Interceptors;
using Autogestor.Infrastructure.Persistence.Repositories;
using Autogestor.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Autogestor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
