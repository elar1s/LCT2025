using StoreService.Services;

namespace StoreService.Extensions;

/// <summary>
/// Domain service layer DI registrations.
/// </summary>
public static class DomainServicesExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
        return services;
    }
}

