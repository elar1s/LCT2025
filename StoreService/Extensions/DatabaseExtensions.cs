using Microsoft.EntityFrameworkCore;
using StoreService.Database;

namespace StoreService.Extensions;

/// <summary>
/// Database related DI registrations.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Registers the EF Core DbContext (PostgreSQL).
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("StoreDb")));
        return services;
    } 
}

