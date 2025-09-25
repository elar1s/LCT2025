using StoreService.Repositories;

namespace StoreService.Extensions;

/// <summary>
/// Repository & UnitOfWork registrations.
/// </summary>
public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Open generic registration for repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        // Unit of work (will receive repositories via constructor injection)
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
