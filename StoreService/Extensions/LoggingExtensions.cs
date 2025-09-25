using Serilog;

namespace StoreService.Extensions;

/// <summary>
/// Logging related extensions (Serilog configuration).
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Adds Serilog configuration for the host using appsettings.json (Serilog section).
    /// </summary>
    public static IHostBuilder AddSerilogLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((ctx, services, cfg) =>
        {
            cfg.ReadFrom.Configuration(ctx.Configuration)
               .ReadFrom.Services(services)
               .Enrich.FromLogContext();
        });
        return hostBuilder;
    }
}

